using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GalleryBI.Tests
{
    [TestClass]
    public class EmailTests
    {
        private ILogger logger;

        public EmailTests()
        {
            logger = new TestConsoleLogge();
        }

        [TestMethod]
        public void EmailGenerateAndSenderTests()
        {
            EmailMetadata emailMetadata = new EmailMetadata()
            {
                IssueCatalog = IssueCatalogs.Low,
                IssueDate = DateTime.UtcNow.ToString(),
                IssueLink = "https://github.com/Azure-Samples/openai-chat-vision-quickstart/issues/5",
                OwnerEmailList = "lianw@microsoft.com",
                Repo = "https://github.com/Azure-Samples/openai-chat-vision-quickstart",
                EmailTemplateCatalogs = EmailTemplateCatalogs.FirstFixNotification
            };
            var email1 = emailMetadata.GenerateEmail();

            var sender = new EmailSender(logger);
            sender.SendEmailAsync(email1).Wait();
            //sender.SendEmailAsync(email2).Wait();

            //emailMetadata.IssueCatalog = IssueCatalogs.High;
            //var email3 = emailMetadata.GenerateEmail();
            //sender.SendEmailAsync(email3).Wait();
        }
    }
}
