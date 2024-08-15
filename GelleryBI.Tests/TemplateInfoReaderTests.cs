using GalleryBI;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GelleryBI.Tests
{
    [TestClass]
    public class TemplateInfoReaderTests
    {
        private ILogger logger;

        public TemplateInfoReaderTests()
        {
            logger = new TestConsoleLogge();
        }

        [TestMethod]
        public void TemplateInfoReaderPassTest()
        {
            var reader = new TemplateInfoReader(logger);
            var result = reader.ReadAsync().Result;
            Assert.AreEqual(22, result.Count());
        }
    }
}
