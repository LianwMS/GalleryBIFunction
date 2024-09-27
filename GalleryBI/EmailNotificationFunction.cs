using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GalleryBI
{
    public class EmailNotificationFunction
    {
        private readonly ILogger logger;


        public EmailNotificationFunction(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<EmailNotificationFunction>();
        }

        [Function("Function")]
        public void Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer)
        {
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            // Query the tempalte issue
            var templateData = new List<Template>();//new TemplateInfoReader(logger).ReadAsync().Result;

            // Generate the email content
            var emailList = new List<Email>();
            var candidateEmailTemplate = templateData.Where(t => t.ValidationActiveIssues != null && t.ValidationActiveIssues.Any()).ToList();

            // For Testing
            candidateEmailTemplate.Add(new Template()
            {
                Url = "https://github.com/Azure-Samples/contoso-chat",
                Name = "Azure-Samples/contoso-chat",
                ValidationActiveIssues = new List<string>() { "https://github.com/Azure-Samples/contoso-chat/issues/142" }
            });

            for (int i = 0; i < candidateEmailTemplate.Count; i++)
            {
                var template = candidateEmailTemplate[i];
                var email = new Email()
                {
                    To = "lianw@microsoft.com",
                    Subject = NotificationEmailTemplate.SUBJECT,
                    Body = NotificationEmailTemplate.GetBody(
                        repo: template.Url,
                        galleryUrl: "AI Gallery",
                        issueDate: DateTimeOffset.UtcNow.Date.ToString(),
                        issueLink: template.ValidationActiveIssues?.FirstOrDefault(),
                        teamEmail: "AI Gallery Email",
                        timeFrame: "7 days")
                };
                emailList.Add(email);
            }

            // For Testing
            //emailList.Clear();

            // Send the email in parallel
            var emailSender = new EmailSender(logger);
            var tasks = new List<Task>();
            try
            {
                for (int j = 0; j < emailList.Count; j++)
                {
                    var email = emailList[j];
                    Task task = Task.Run(async () =>
                    {                        
                        await emailSender.SendEmailAsync(email).ConfigureAwait(false);
                    });

                    tasks.Add(task);
                }
                Task.WhenAll(tasks.ToArray()).Wait();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Error in sending email, with {exception.Message}");
            }

            if (myTimer.ScheduleStatus is not null)
            {
                logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
