using System.Net.Http.Headers;
using Octokit;

namespace GalleryBI
{
    public class GithubHelper
    {
        public static async Task<string> GetTemplateOwner(GitHubClient githubClient, string? templateUrl)
        {
            var ownerEmails = string.Empty;
            if (!string.IsNullOrEmpty(templateUrl))
            {
                // Find Owner file
                // Find latest committer
                (string owner, string repoName) = GetRepoOwnerAndName(templateUrl);
                var contributors = await githubClient.Repository.GetAllContributors(owner, repoName).ConfigureAwait(false);

                var selectFromContributorsNum = 5;
                if (contributors.Count == 0)
                {
                    ownerEmails = AppContext.TemplateDefaultOwner;
                }
                else if (contributors.Count < selectFromContributorsNum)
                {
                    selectFromContributorsNum = contributors.Count;
                }

                for (int i = 0; i < selectFromContributorsNum; i++)
                {
                    var contributor = await githubClient.User.Get(contributors[i].Login).ConfigureAwait(false);
                    var contributorEmail = contributor.Email;
                    if (!string.IsNullOrEmpty(contributorEmail))
                    {
                        ownerEmails = string.IsNullOrEmpty(ownerEmails) ? contributorEmail : ownerEmails + ";" + contributorEmail;
                    }
                }
            }

            if (string.IsNullOrEmpty(ownerEmails))
            {
                ownerEmails = AppContext.TemplateDefaultOwner;
            }

            return ownerEmails;
        }

        public static async Task<IssueCatalogs> GetIssueCatalog(GitHubClient githubClient, string? issueUrl)
        {
            var result = IssueCatalogs.Moderate;
            if (!string.IsNullOrWhiteSpace(issueUrl))
            {
                (string owner, string repoName, int issueId) = ParseIssueUrl(issueUrl);
                var issue = await githubClient.Issue.Get(owner, repoName, issueId).ConfigureAwait(false);
                if (issue.Body != null)
                {
                    if (issue.Body.Contains("Severity: High", System.StringComparison.OrdinalIgnoreCase))
                    {
                        result = IssueCatalogs.High;
                    }
                    else if (issue.Body.Contains("Severity: Low", System.StringComparison.OrdinalIgnoreCase))
                    {
                        result = IssueCatalogs.Low;
                    }
                    else if (issue.Body.Contains("Severity: Moderate", System.StringComparison.OrdinalIgnoreCase))
                    {
                        result = IssueCatalogs.Moderate;
                    }
                }
            }
            return result;
        }

        public static async Task<string> GetFileContextFromGithubRepo(string fileUrl, string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                // Add the Authorization header with the access token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.raw+json"));
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                // Send the GET request
                HttpResponseMessage response = await httpClient.GetAsync(fileUrl);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string jsonContent = await response.Content.ReadAsStringAsync();

                return jsonContent;
            }
        }

        public static (string Owner, string RepoName) GetRepoOwnerAndName(string? repoUrl)
        {
            string owner = string.Empty;
            string repoName = string.Empty;

            if (!string.IsNullOrWhiteSpace(repoUrl))
            {
                // Create a Uri object from the repo URL
                var uri = new Uri(repoUrl);

                // Split the path segments
                var segments = uri.AbsolutePath.Trim('/').Split('/');

                // Ensure the URL has the expected structure
                if (segments.Length < 2)
                {
                    throw new ArgumentException("The provided URL does not seem to be a valid GitHub repository URL.");
                }

                // Extract the owner and repo name from the URL segments
                owner = segments[0];
                repoName = segments[1];
            }

            return (owner, repoName);
        }

        public static (string ownerName, string repoName, int issueId) ParseIssueUrl(string issueUrl)
        {
            // Ensure the URL is well-formed
            if (Uri.TryCreate(issueUrl, UriKind.Absolute, out Uri? uri))
            {
                // Split the path segments
                string[] segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                // Validate the URL format
                if (segments.Length >= 4 && segments[2] == "issues")
                {
                    string ownerName = segments[0];
                    string repoName = segments[1];

                    // Try to parse the issue ID
                    if (int.TryParse(segments[3], out int issueId))
                    {
                        return (ownerName, repoName, issueId);
                    }
                    else
                    {
                        throw new ArgumentException("Issue ID is not a valid integer.");
                    }
                }
                else
                {
                    throw new ArgumentException("The provided URL does not follow the expected GitHub issue URL format.");
                }
            }
            else
            {
                throw new ArgumentException("The provided string is not a valid URL.");
            }
        }
    }
}
