using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;

namespace GalleryBI
{
    public class TemplateInfoReader
    {
        private readonly ILogger logger;
        private readonly GitHubClient githubClient;

        public TemplateInfoReader(ILogger logger)
        {
            this.logger = logger;

            var accessToken = AzureHelper.GetSecretFromKV(AppContext.BIKVUri, AppContext.AzurePATSecretName).Result;
            this.githubClient = new GitHubClient(new ProductHeaderValue("TemplateInfoReader"));
            githubClient.Credentials = new Credentials(accessToken);
        }

        public async Task<IEnumerable<Template>> ReadAsync()
        {
            var runTime = DateTime.UtcNow;
            //var fileContent = await ReaderHelper.GetFileContextFromGithubRepo(AppContext.TemplateFileUri, accessToken);

            var contentInBytes = await this.githubClient.Repository.Content.GetRawContent(AppContext.GithubRepoOwner, AppContext.GithubRepoName, AppContext.TemplateFilePath).ConfigureAwait(false);
            var fileContent = Encoding.Default.GetString(contentInBytes);

            List<Template> templates = JsonConvert.DeserializeObject<List<Template>>(fileContent) ?? new List<Template>();

            for (int i = 0; i < templates.Count; i++)
            {
                var template = templates[i];
                template.TimeStamp = runTime;
                template.Url = template.Source;
                template.Catalog = TemplateCatalogs.AI;

                var (owner, repoName) = GithubHelper.GetRepoOwnerAndName(template.Source);

                var templateInfo = await this.githubClient.Repository.Get(owner, repoName).ConfigureAwait(false);
                template.Name = templateInfo.FullName;
                template.Watch = templateInfo.SubscribersCount;
                template.Star = templateInfo.StargazersCount;
                template.Fork = templateInfo.ForksCount;
                template.Topics = templateInfo.Topics.ToList();

                var repoIssueRequest = new RepositoryIssueRequest()
                {
                    Creator = "ai-apps-bot",
                    State = ItemStateFilter.All
                };
                var issues = await this.githubClient.Issue.GetAllForRepository(owner, repoName, repoIssueRequest).ConfigureAwait(false);
                template.ValidationActiveIssues = issues.Where(i => i.State == ItemState.Open).Select(i => i.HtmlUrl).ToList();
                template.ValidationNonActiveIssues = issues.Where(i => i.State != ItemState.Open).Select(i => i.HtmlUrl).ToList();

            }
            this.logger.LogInformation("Templates count: " + templates.Count);
            this.logger.LogInformation("Templates data read successfully.");
            return templates;
        }
    }
}
