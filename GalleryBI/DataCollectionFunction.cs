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

            var templateData = new TemplateInfoReader(logger).ReadAsync().Result;
            logger.LogInformation("Template data read successfully.");
            var validationData = new ValidationReader(logger).ReadAsync().Result;
            logger.LogInformation("Validation data read successfully.");

            var templateWriter = new TemplateInfoWriter(AppContext.ClusterUri, AppContext.BIDBName, AppContext.TemplateInfoTableName, TemplateMappingInfo.Name, TemplateMappingInfo.Mapping, logger);
            templateWriter.WriteAsync(templateData).Wait();            

            var validationWriter = new ValidationInfoWriter(AppContext.ClusterUri, AppContext.BIDBName, AppContext.ValidationInforTableName, ValidationMappingInfo.Name, ValidationMappingInfo.Mapping, logger);
            var inputData = validationWriter.RemoveDup(validationData).Result;
            validationWriter.WriteAsync(inputData).Wait();



            if (myTimer.ScheduleStatus is not null)
            {
                logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
