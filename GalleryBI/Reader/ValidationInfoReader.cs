using Microsoft.Extensions.Logging;
using Octokit;

namespace GalleryBI
{
    public class ValidationInfoReader
    {
        private readonly ILogger logger;
        private readonly GitHubClient githubClient;

        public ValidationInfoReader(ILogger logger)
        {
            this.logger = logger;

            var accessToken = AzureHelper.GetSecretFromKV(AppContext.BIKVUri, AppContext.AzurePATSecretName).Result;
            this.githubClient = new GitHubClient(new ProductHeaderValue("ValidationInfoReader"));
            githubClient.Credentials = new Credentials(accessToken);
        }

        public async Task<IEnumerable<Validation>> ReadAsync()
        {
            var functionRunTime = DateTime.UtcNow;
            var validations = new List<Validation>();

            var validationWorkflow = await this.githubClient.Actions.Workflows.Get(AppContext.ValidationRepoOwner, AppContext.ValidationRepoName, AppContext.ValidationWorkflowFileName);
            if (validationWorkflow != null)
            {
                var request = new WorkflowRunsRequest()
                {
                    Status = CheckRunStatusFilter.Completed,
                    Created = string.Format(">{0}", functionRunTime.Date.AddDays(-7).ToString("yyyy-MM-dd"))
                };
                var candidates = await this.githubClient.Actions.Workflows.Runs.ListByWorkflow(AppContext.ValidationRepoOwner, AppContext.ValidationRepoName, validationWorkflow.Id, request);
                this.logger.LogInformation("Runs count: " + candidates.TotalCount);

                for (int i = 0; i < candidates.TotalCount; ++i)
                {
                    var jobs = await this.githubClient.Actions.Workflows.Jobs.List(AppContext.ValidationRepoOwner, AppContext.ValidationRepoName, candidates.WorkflowRuns[i].Id);
                    for (int j = 0; j < jobs.TotalCount; ++j)
                    {
                        if (jobs.Jobs[j].Name.StartsWith("https"))
                        {
                            var validation = new Validation()
                            {
                                TimeStamp = functionRunTime,
                                RunTime = jobs.Jobs[j].StartedAt.DateTime,
                                Url = jobs.Jobs[j].Name,
                                Result = jobs.Jobs[j].Conclusion.ToString(),
                                WorkflowRepoName = validationWorkflow.Name,
                                WorkflowName = candidates.WorkflowRuns[i].Name,
                                RunId = jobs.Jobs[j].RunId.ToString(),
                                JobId = jobs.Jobs[j].Id.ToString(),
                                RunStartTime = candidates.WorkflowRuns[i].CreatedAt.DateTime,
                                AttemptId = candidates.WorkflowRuns[i].RunAttempt.ToString()
                            };
                            if (jobs.Jobs[j].Conclusion == WorkflowJobConclusion.Failure)
                            {
                                // Get Step Log?
                                var log = await this.githubClient.Actions.Workflows.Jobs.GetLogs(AppContext.ValidationRepoOwner, AppContext.ValidationRepoName, jobs.Jobs[j].Id);
                                validation.Details = log;
                                List<string> logs = log.Split("\n").ToList();
                                int pos = logs.FindIndex(x => x.EndsWith("Validation failed, creating issue"));
                                if (pos != -1)
                                {
                                    var issueLogs = logs[pos + 1].Split(" ").ToList();
                                    if (issueLogs.Count > 1)
                                    {
                                        var issue = issueLogs[1];
                                        if (issue.StartsWith("https"))
                                        {
                                            validation.Details = issue;
                                        }
                                    }
                                }
                            }
                            validations.Add(validation);
                        }
                    }
                }
            }

            this.logger.LogInformation("Validations count: " + validations.Count);
            this.logger.LogInformation("Validations data read successfully.");
            return validations;
        }
    }
}
