using System.Net.Http.Headers;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace GalleryBI
{
    internal class ReaderHelper
    {
        public static async Task<string> GetSecretFromKV(string keyVaultUri, string secretName)
        {
            try
            {
                // Create a SecretClient with DefaultAzureCredential, which uses Managed Identity by default
                var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

                // Retrieve the secret
                KeyVaultSecret secret = await client.GetSecretAsync(secretName);

                // Return the secret value
                return secret.Value;
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, rethrow, etc.)
                Console.WriteLine($"Error retrieving secret from Azure Key Vault: {ex.Message}");
                throw;
            }
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

    }
}
