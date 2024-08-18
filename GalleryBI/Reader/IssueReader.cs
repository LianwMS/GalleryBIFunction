using Microsoft.Extensions.Logging;
using Octokit;

namespace GalleryBI.Reader
{
    public class IssueReader
    {
        private readonly ILogger logger;
        private readonly GitHubClient githubClient;

        public IssueReader(ILogger logger)
        {
            this.logger = logger;

            var accessToken = ReaderHelper.GetSecretFromKV(AppContext.BIKVUri, AppContext.AzurePATSecretName).Result;
            this.githubClient = new GitHubClient(new ProductHeaderValue("IssueReader"));
            githubClient.Credentials = new Credentials(accessToken);
        }

        public async Task<IEnumerable<Issue>> ReadAsync(IEnumerable<string> currentOpenIssues, IEnumerable<string> createdIn7DaysIssues)
        {
            var currentTime = DateTime.UtcNow;
            var issueList = currentOpenIssues.Union(createdIn7DaysIssues).ToList();
            var result = new List<Issue>();

            for (int i = 0; i < issueList.Count; ++i)
            {
                var (owner, repoName, issueId) = ReaderHelper.ParseIssueUrl(issueList[i]);
                var issue = githubClient.Issue.Get(owner, repoName, issueId);
                var tempIssue = new Issue()
                {
                    TimeStamp = currentTime,
                    Id = issue.Result.Id.ToString(),
                    TemplateName = $"{owner}/{repoName}",
                    Url = issue.Result.Url,
                    Status = issue.Result.State.ToString(),
                    CreatedAt = issue.Result.CreatedAt.UtcDateTime,
                    ClosedAt = issue.Result.ClosedAt?.UtcDateTime ?? null,
                    Title = issue.Result.Title,
                };
            }

            await Task.Delay(1000);
            return result;
        }
    }
}
