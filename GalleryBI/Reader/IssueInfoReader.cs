using Microsoft.Extensions.Logging;
using Octokit;

namespace GalleryBI
{
    public class IssueInfoReader
    {
        private readonly ILogger logger;
        private readonly GitHubClient githubClient;

        public IssueInfoReader(ILogger logger)
        {
            this.logger = logger;

            var accessToken = AzureHelper.GetSecretFromKV(AppContext.BIKVUri, AppContext.AzurePATSecretName).Result;
            this.githubClient = new GitHubClient(new ProductHeaderValue("IssueReader"));
            githubClient.Credentials = new Credentials(accessToken);
        }

        public async Task<IEnumerable<Issue>> ReadAsync(IEnumerable<string> issueUrlList)
        {
            var currentTime = DateTime.UtcNow;
            var result = new List<Issue>();

            using (IEnumerator<string> enumerator = issueUrlList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var issueUrl = enumerator.Current;
                    var (owner, repoName, issueId) = GithubHelper.ParseIssueUrl(issueUrl);
                    var issue = githubClient.Issue.Get(owner, repoName, issueId);
                    var tempIssue = new Issue()
                    {
                        TimeStamp = currentTime,
                        Id = issue.Result.Id.ToString(),
                        TemplateName = $"{owner}/{repoName}",
                        Url = issue.Result.HtmlUrl,
                        Status = issue.Result.State.ToString(),
                        CreatedAt = issue.Result.CreatedAt.UtcDateTime,
                        ClosedAt = issue.Result.ClosedAt?.UtcDateTime ?? null,
                        Title = issue.Result.Title,
                    };
                    result.Add(tempIssue);
                }
            }

            this.logger.LogInformation("Issues count: " + result.Count);
            this.logger.LogInformation("Issues data read successfully");
            return result;
        }
    }
}
