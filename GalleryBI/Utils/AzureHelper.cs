using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace GalleryBI
{
    public class AzureHelper
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
    }
}
