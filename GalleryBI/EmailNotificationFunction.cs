using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GalleryBI
{
    public class EmailNotificationFunction
    {
        private readonly ILogger logger;
        private readonly GitHubClient githubClient;
        private readonly TimeSpan FIRST_EMAIL_DURATION_START = new TimeSpan(0, 0, 0, 0);
        private readonly TimeSpan FIRST_EMAIL_DURATION_END = new TimeSpan(1, 0, 0, 0);
        private readonly TimeSpan LAST_EMAIL_DURATION_START = new TimeSpan(6, 0, 0, 0);
        private readonly TimeSpan LAST_EMAIL_DURATION_END = new TimeSpan(7, 0, 0, 0);

        public EmailNotificationFunction(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<EmailNotificationFunction>();
            var accessToken = AzureHelper.GetSecretFromKV(AppContext.BIKVUri, AppContext.AzurePATSecretName).Result;
            this.githubClient = new GitHubClient(new ProductHeaderValue("ValidationInfoReader"));
            githubClient.Credentials = new Credentials(accessToken);
        }

        [Function("EmailNotificationFunction")]
        public void Run([TimerTrigger("0 30 */12 * * *")] TimerInfo myTimer)
        {
            var emailInfoWriter = new EmailInfoWriter(AppContext.ClusterUri, AppContext.BIDBName, AppContext.EmailTableName, EmailMappingInfo.Name, EmailMappingInfo.Mapping, logger);
            var now = DateTime.UtcNow;
            logger.LogInformation($"C# Timer trigger function executed at: {now}");

            // Query the tempalte issue
            var templates = new TemplateInfoReader(logger).ReadAsync().Result;
            var templatesForEmail = templates.Where(t => t.ValidationActiveIssues != null && t.ValidationActiveIssues.Any()).ToList();

            //// For Testing
            //templatesForEmail.Add(new Template()
            //{
            //    Url = "https://github.com/LianwMS/GalleryBIFunction",
            //    Name = "Azure-Samples/azure-search-openai-demo-java",
            //    ValidationActiveIssues = new List<string>() { "https://github.com/microsoft/template-validation-action/issues/46" }
            //});

            // Generate the email list and unpublish list
            var unpublishList = new List<string>();
            var emailList = new List<Email>();
            for (int i = 0; i < templatesForEmail.Count; i++)
            {
                var template = templatesForEmail[i];
                var ownerEmail = GithubHelper.GetTemplateOwner(githubClient, template.Url).Result;
                var issueList = templatesForEmail[i].ValidationActiveIssues ?? new List<string>();

                for (int j = 0; j < issueList.Count; ++j)
                {
                    var (owner, repoName, issueId) = GithubHelper.ParseIssueUrl(issueList[j]);
                    var issue = githubClient.Issue.Get(owner, repoName, issueId).Result;
                    var duration = now - issue.CreatedAt.UtcDateTime;
                    var issueCatalog = GithubHelper.GetIssueCatalog(githubClient, issue.HtmlUrl).Result;

                    if (issue.State == ItemState.Open)
                    {
                        if (duration >= FIRST_EMAIL_DURATION_START && duration <= FIRST_EMAIL_DURATION_END)
                        {
                            var metadate = new EmailMetadata()
                            {
                                Repo = template.Url ?? string.Empty,
                                IssueDate = issue.CreatedAt.UtcDateTime.ToString(),
                                IssueLink = issue.HtmlUrl,
                                IssueCatalog = issueCatalog,
                                OwnerEmailList = ownerEmail,
                                EmailTemplateCatalogs = EmailTemplateCatalogs.FirstFixNotification

                            };

                            emailList.Add(metadate.GenerateEmail());
                        }
                        else if (duration >= LAST_EMAIL_DURATION_START && duration <= LAST_EMAIL_DURATION_END && issueCatalog != IssueCatalogs.High)
                        {
                            var metadate = new EmailMetadata()
                            {
                                Repo = template.Url ?? string.Empty,
                                IssueDate = issue.CreatedAt.UtcDateTime.ToString(),
                                IssueLink = issue.HtmlUrl,
                                IssueCatalog = issueCatalog,
                                OwnerEmailList = ownerEmail,
                                EmailTemplateCatalogs = EmailTemplateCatalogs.LastFixNotification
                            };

                            emailList.Add(metadate.GenerateEmail());
                        }

                        if ((issueCatalog == IssueCatalogs.High && duration > LAST_EMAIL_DURATION_END)
                            || (duration > LAST_EMAIL_DURATION_END))
                        {
                            unpublishList.Add(template.Url);
                        }
                    }
                }
            }

            logger.LogInformation("{0} emails. Remove dup...", emailList.Count);
            var emailInsertList = emailInfoWriter.RemoveDup(emailList).Result.ToList();

            // Send the email in parallel
            logger.LogInformation("Total {0} emails need to be sent.", emailInsertList.Count);
            var emailSender = new EmailSender(logger);
            var tasks = new List<Task>();
            try
            {
                for (int j = 0; j < emailInsertList.Count; j++)
                {
                    var email = emailInsertList[j];
                    Task task = Task.Run(async () =>
                    {
                        await emailSender.SendEmailAsync(email).ConfigureAwait(false);
                        await emailInfoWriter.WriteAsync(new List<Email>() { email }).ConfigureAwait(false);
                    });

                    tasks.Add(task);
                }
                Task.WhenAll(tasks.ToArray()).Wait();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Error in sending email, with {exception.Message}");
            }

            // For Testing
            // unpublishList.Add("https://github.com/Azure-Samples/azure-search-openai-demo");

            // Unpublish the template
            logger.LogInformation("Total {0} tempaltes need to be unpublished.", unpublishList.Count);
            for (int i = 0; i < unpublishList.Count; i++)
            {
                var dispatch = new CreateWorkflowDispatch("main");
                dispatch.Inputs = new Dictionary<string, object>()
                {
                    { "repositoryUrl", unpublishList[i] }
                };
                githubClient.Actions.Workflows.CreateDispatch(AppContext.UnpublishWFRepoOwner, AppContext.UnpublishWFRepoName, AppContext.UnpublishWFName, dispatch).Wait();
            }

            if (myTimer.ScheduleStatus is not null)
            {
                logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
