using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GalleryBI
{
    public class EmailSender
    {
        private static HttpClient client = new HttpClient();
        private readonly ILogger logger;

        public EmailSender(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task SendEmailAsync(Email email)
        {
            var jsonData = JsonConvert.SerializeObject(email);
            logger.LogInformation("Email data: " + jsonData);

            HttpResponseMessage result = await client.PostAsync(AppContext.EmailTriggerUrl, new StringContent(jsonData, Encoding.UTF8, "application/json"));
            var statusCode = result.StatusCode;

            if (statusCode == HttpStatusCode.Accepted)
            {
                logger.LogInformation("Email sent successfully.");
            }
            else
            {
                logger.LogError($"Email sending failed. Status code: {statusCode}, {result.Content.ToString()}");
            }
        }
    }
}
