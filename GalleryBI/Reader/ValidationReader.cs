using Microsoft.Extensions.Logging;
using Octokit;

namespace GalleryBI
{
    public class ValidationReader
    {
        private readonly ILogger logger;
        private readonly GitHubClient githubClient;

        public ValidationReader(ILogger logger)
        {
            this.logger = logger;

            var accessToken = ReaderHelper.GetSecretFromKV(AppContext.BIKVUri, AppContext.Hund030PATSecretName).Result;
            this.githubClient = new GitHubClient(new ProductHeaderValue("ValidationInfoReader"));
            githubClient.Credentials = new Credentials(accessToken);
        }

        public async Task<IEnumerable<Validation>> ReadAsync()
        {
            var functionRunTime = DateTime.UtcNow;
            var validations = new List<Validation>();
            var workflow = await this.githubClient.Actions.Workflows.List(AppContext.ValidationRepoOwner, AppContext.ValidationRepoName);
            var workflowRepoFullName = AppContext.ValidationRepoOwner + "/" + AppContext.ValidationRepoName;
            if (workflow != null && workflow.TotalCount > 0)
            {
                this.logger.LogInformation("Workflow count: " + workflow.TotalCount);
                var id = workflow.Workflows?.FirstOrDefault<Workflow>(Workflow => Workflow.Name == AppContext.ValidationWorkflowName)?.Id;

                if (id != null)
                {
                    var request = new WorkflowRunsRequest()
                    {
                        Status = CheckRunStatusFilter.Completed,
                        Created = string.Format(">{0}", functionRunTime.Date.AddDays(-7).ToString("yyyy-MM-dd"))
                    };
                    var candidates = await this.githubClient.Actions.Workflows.Runs.ListByWorkflow(AppContext.ValidationRepoOwner, AppContext.ValidationRepoName, id.Value, request);
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
                                    WorkflowRepoName = workflowRepoFullName,
                                    WorkflowName = candidates.WorkflowRuns[i].Name,
                                    RunId = jobs.Jobs[j].RunId.ToString(),
                                    JobId = jobs.Jobs[j].Id.ToString()
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
            }

            this.logger.LogInformation("Validation count: " + validations.Count);
            return validations;
        }
    }
}
