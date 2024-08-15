using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GalleryBI
{
    public class DataCollectionFunction
    {
        private readonly ILogger logger;

        public DataCollectionFunction(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<DataCollectionFunction>();
        }

        [Function("TestInsertFunction")]
        public void Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer)
        {
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var data = new TemplateInfoReader(logger).ReadAsync().Result;
            logger.LogInformation("Data read successfully.");

            var templateWriter = new TemplateInfoWriter(AppContext.ClusterUri, AppContext.BIDBName, AppContext.TemplateInfoTableName, TemplateMappingInfo.Name, TemplateMappingInfo.Mapping, logger);
            logger.LogInformation("Start to ingest data.");
            templateWriter.WriteAsync(data).Wait();
            logger.LogInformation("Data ingested successfully.");

            if (myTimer.ScheduleStatus is not null)
            {
                logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
