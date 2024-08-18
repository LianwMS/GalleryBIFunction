﻿using GalleryBI;
using Microsoft.Extensions.Logging;

namespace GelleryBI.Tests
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
    }
}
