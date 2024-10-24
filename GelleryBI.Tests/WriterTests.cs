using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GalleryBI.Tests
{
    [TestClass]
    public class WriterTests
    {
        private ILogger logger;

        public WriterTests()
        {
            logger = new TestConsoleLogge();
        }

        [TestMethod]
        public void TemplateInfoWriterTest()
        {
            var writer = new TemplateInfoWriter(TestAppContext.ClusterUri, TestAppContext.BIDBName, TestAppContext.TemplateInfoTableName, TemplateMappingInfo.Name, TemplateMappingInfo.Mapping, logger);
            var data = new List<Template>()
            {
                new Template()
                {
                    TimeStamp = DateTime.UtcNow,
                    Catalog = "Catalog",
                    Name = "Name",
                    Url = "Url",
                    Tags = new List<string>() { "Tag1", "Tag2" },
                    Star = 1,
                    Fork = 1,
                    Watch = 1,
                    Vistor = 1,
                    Clone = 1,
                    Topics = new List<string>() { "Topic1", "Topic2" },
                    Languages = new List<string>() { "Language1", "Language2" },
                    Models = new List<string>() { "Model1", "Model2" },
                    AzureServices = new List<string>() { "AzureService1", "AzureService2" },
                    Types = new List<string>() { "Type1", "Type2" },
                    Source = "Source",
                    ValidationActiveIssues = new List<string>() { "ValidationActiveIssue1", "ValidationActiveIssue2" },
                    ValidationNonActiveIssues = new List<string>() { "ValidationNonActiveIssue1", "ValidationNonActiveIssue2" }
                }
            };
            writer.WriteAsync(data).Wait();
        }

        [TestMethod]
        public void ValidationInfoWriterTest()
        {
            var writer = new ValidationInfoWriter(TestAppContext.ClusterUri, TestAppContext.BIDBName, TestAppContext.ValidationInfoTableName, ValidationMappingInfo.Name, ValidationMappingInfo.Mapping, logger);
            var data = new List<Validation>()
            {
                new Validation()
                {
                    TimeStamp = DateTime.UtcNow,
                    RunTime = DateTime.UtcNow,
                    Details = "Details",
                    Result = "Result",
                    Url = "Url",
                    WorkflowName = "WorkflowName",
                    WorkflowRepoName = "WorkflowRepoName",
                    RunId = "12345",
                    JobId = "12345",
                    RunStartTime= DateTime.UtcNow,
                    AttemptId = "12345"
                }
            };
            writer.WriteAsync(data).Wait();
        }

        [TestMethod]
        public void ValidationInfoRemoveDupTest()
        {
            var writer = new ValidationInfoWriter(TestAppContext.ClusterUri, TestAppContext.BIDBName, TestAppContext.ValidationInfoTableName, ValidationMappingInfo.Name, ValidationMappingInfo.Mapping, logger);
            var data = new List<Validation>()
            {
                new Validation()
                {
                    TimeStamp = DateTime.UtcNow,
                    RunTime = DateTime.UtcNow,
                    Details = "Details",
                    Result = "Result",
                    Url = "Url",
                    WorkflowName = "WorkflowName",
                    WorkflowRepoName = "WorkflowRepoName",
                    RunId = "12345",
                    JobId = "12345",
                }
            };

            var dataRemoveDup = writer.RemoveDup(data).Result;
        }

        [TestMethod]
        public void IssueInfoWriterTest()
        {
            var writer = new IssueInfoWriter(TestAppContext.ClusterUri, TestAppContext.BIDBName, TestAppContext.IssueInfoTableName, IssueMappingInfo.Name, IssueMappingInfo.Mapping, logger);
            var data = new List<Issue>()
            {
                new Issue()
                {
                    TimeStamp = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    ClosedAt = null,
                    Id = "12345",
                    Status = "Open",
                    TemplateName = "TemplateName",
                    Title = "Title",
                    Url = "Url",
                }
            };
            writer.WriteAsync(data).Wait();
        }

        [TestMethod]
        public void EmailInfoWriterTest()
        {
            var randomIssueLink = "IssueLink" + Guid.NewGuid().ToString();
            var emailMetadata = new EmailMetadata()
            {
                Repo = "https://github.com/LianwMS/GalleryBIFunction",
                IssueLink = randomIssueLink,
                IssueCatalog = IssueCatalogs.High,
                IssueDate = DateTime.UtcNow.ToString(),
                OwnerEmailList = "lianw@microsoft.com",
                EmailTemplateCatalogs = EmailTemplateCatalogs.FirstFixNotification,
            };
            var email = emailMetadata.GenerateEmail();
            var data = new List<Email>();
            data.Add(email);

            var writer = new EmailInfoWriter(TestAppContext.ClusterUri, TestAppContext.BIDBName, TestAppContext.EmailTableName, EmailMappingInfo.Name, EmailMappingInfo.Mapping, logger);

            var dataInsert = writer.RemoveDup(data).Result;
            Assert.AreEqual(1, dataInsert.Count());

            writer.WriteAsync(data).Wait();
            var dataInsertAgain = writer.RemoveDup(data).Result;
            Assert.AreEqual(0, dataInsertAgain.Count());
        }
    }
}
