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

        [Function("DataCollectionFunction")]
        public void Run([TimerTrigger("0 0 */12 * * *")] TimerInfo myTimer)
        {
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var templateData = new TemplateInfoReader(logger).ReadAsync().Result;

            var validationData = new ValidationInfoReader(logger).ReadAsync().Result;

            var openedIssues = templateData
                .Where(t => t.ValidationActiveIssues != null)
                .SelectMany(t => t.ValidationActiveIssues ?? new List<string>());

            var closedIssues = templateData
                .Where(t => t.ValidationNonActiveIssues != null)
                .SelectMany(t => t.ValidationNonActiveIssues ?? new List<string>());

            var openedIn7DaysIssues = validationData
                .Where(t => t.Details != null && t.Details.StartsWith("https"))
                .Select(t => t.Details);

            var issuesUrlList = openedIssues.Union(closedIssues).Union(openedIn7DaysIssues).ToList();
            var issueData = new IssueInfoReader(logger).ReadAsync(issuesUrlList).Result;

            var templateWriter = new TemplateInfoWriter(AppContext.ClusterUri, AppContext.BIDBName, AppContext.TemplateInfoTableName, TemplateMappingInfo.Name, TemplateMappingInfo.Mapping, logger);
            templateWriter.WriteAsync(templateData).Wait();

            var validationWriter = new ValidationInfoWriter(AppContext.ClusterUri, AppContext.BIDBName, AppContext.ValidationInforTableName, ValidationMappingInfo.Name, ValidationMappingInfo.Mapping, logger);
            var inputData = validationWriter.RemoveDup(validationData).Result;
            validationWriter.WriteAsync(inputData).Wait();

            var issueWriter = new IssueInfoWriter(AppContext.ClusterUri, AppContext.BIDBName, AppContext.IssueInfoTableName, IssueMappingInfo.Name, IssueMappingInfo.Mapping, logger);
            issueWriter.WriteAsync(issueData).Wait();

            if (myTimer.ScheduleStatus is not null)
            {
                logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
