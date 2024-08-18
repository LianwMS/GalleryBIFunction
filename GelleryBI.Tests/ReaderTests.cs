using GalleryBI;
using Microsoft.Extensions.Logging;

namespace GelleryBI.Tests
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
            var reader = new ValidationReader(logger);
            var result = reader.ReadAsync().Result;
            Assert.AreNotEqual(-1, result.Count());
        }

        public void IssueReaderTest()
        {
            var templateReader = new TemplateInfoReader(logger);
            var templates = templateReader.ReadAsync().Result;

            var validationReader = new ValidationReader(logger);
            var validations = validationReader.ReadAsync().Result;

            List<string> allValidationActiveIssues = templates
            .Where(t => t.ValidationActiveIssues != null)
            .SelectMany(t => t.ValidationActiveIssues ?? new List<string>())
            .Distinct()
            .ToList();

            if (validations != null)
            {
                List<string> openedIn7DaysIssues = validations
                    .Where(t => t.Details != null && t.Details.StartsWith("https"))
                    .Select(t => t.Details)
                    .Distinct()
                    .ToList();
            }

        }
    }
}
