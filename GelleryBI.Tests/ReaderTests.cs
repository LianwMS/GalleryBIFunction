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
    }
}
