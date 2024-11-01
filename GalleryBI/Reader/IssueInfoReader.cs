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
                    var issue = await githubClient.Issue.Get(owner, repoName, issueId);
                    var tempIssue = new Issue()
                    {
                        TimeStamp = currentTime,
                        Id = issue.Id.ToString(),
                        TemplateName = $"{owner}/{repoName}",
                        Url = issue.HtmlUrl,
                        Status = issue.State.ToString(),
                        CreatedAt = issue.CreatedAt.UtcDateTime,
                        ClosedAt = issue.ClosedAt?.UtcDateTime ?? null,
                        ReopenAt = null,
                        Title = issue.Title,
                        IssueDetails = ParseIssueDetails(issue.Body),
                    };

                    var issueEvents = await githubClient.Issue.Events.GetAllForIssue(owner, repoName, issueId).ConfigureAwait(false);
                    for (int i = 0; i < issueEvents.Count; i++)
                    {
                        if (issueEvents[i].Event == EventInfoState.Reopened)
                        {
                            tempIssue.ReopenAt = issueEvents[i].CreatedAt.UtcDateTime;
                        }
                    }
                    result.Add(tempIssue);
                }
            }

            this.logger.LogInformation("Issues count: " + result.Count);
            this.logger.LogInformation("Issues data read successfully");
            return result;
        }

        public List<string> ParseIssueDetails(string issueBody)
        {
            var result = new List<string>();

            if (issueBody != null)
            {
                var bodyLines = issueBody.Split('\n'); 
                var errorLines = bodyLines.Where(l => l.StartsWith("- [ ]")).ToList();
                foreach (var line in errorLines)
                {
                    foreach (var type in IssueDetailsTypes.All)
                    {
                        if (line.Contains(type, StringComparison.OrdinalIgnoreCase))
                        {
                            result.Add(type);
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
