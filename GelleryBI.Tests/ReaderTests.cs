using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace GalleryBI.Tests
{
    [TestClass]
    public class ReaderTests
    {
        private ILogger logger;

        public ReaderTests()
        {
            logger = new TestConsoleLogge();
        }

        [TestMethod]
        public void TemplateInfoReaderTest()
        {
            var reader = new TemplateInfoReader(logger);
            var result = reader.ReadAsync().Result;
            Assert.AreNotEqual(-1, result.Count());
        }

        [TestMethod]
        public void ValidationReaderTest()
        {
            var reader = new ValidationInfoReader(logger);
            var result = reader.ReadAsync().Result;
            Assert.AreNotEqual(-1, result.Count());
        }

        [TestMethod]
        public void IssueReaderTest()
        {
            var templateReader = new TemplateInfoReader(logger);
            var templates = templateReader.ReadAsync().Result;

            //var validationReader = new ValidationInfoReader(logger);
            //var validations = validationReader.ReadAsync().Result;

            var allValidationActiveIssues = templates
                .Where(t => t.ValidationActiveIssues != null)
                .SelectMany(t => t.ValidationActiveIssues ?? new List<string>()).ToList();

            var allValidationClosedIssues = templates
                .Where(t => t.ValidationNonActiveIssues != null)
                .SelectMany(t => t.ValidationNonActiveIssues ?? new List<string>()).ToList();

            //var openedIn7DaysIssues = validations
            //    .Where(t => t.Details != null && t.Details.StartsWith("https"))
            //    .Select(t => t.Details).ToList();

            var candidate = allValidationActiveIssues.Union(allValidationClosedIssues);//.Union(openedIn7DaysIssues).ToList();

            var issueReader = new IssueInfoReader(logger);
            var issues = issueReader.ReadAsync(candidate).Result;
            Assert.AreNotEqual(-1, issues.Count());
        }

        [TestMethod]
        public void IssueDetailsReaderTest()
        {
            var candidate = new List<string> { "https://github.com/Azure-Samples/summarization-openai-python-promptflow/issues/97" };
            var issueReader = new IssueInfoReader(logger);
            var issues = issueReader.ReadAsync(candidate).Result;
            Assert.AreNotEqual(-1, issues.Count());
        }
    }
}
