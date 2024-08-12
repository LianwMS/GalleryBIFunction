using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Ingestion;
using Kusto.Data.Net.Client;
using Kusto.Ingest;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GalleryBI
{
    public class TestInsertFunction
    {
        private readonly ILogger _logger;
        private string clusterUri = "https://gallerybicluster.eastus.kusto.windows.net";
        private string databaseName = "BIData";
        private string tableName = "TestTemplateInfo";

        public TestInsertFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TestInsertFunction>();
        }

        [Function("TestInsertFunction")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            CreateJsonMappingIfNotExists(new KustoConnectionStringBuilder(clusterUri).WithAadUserPromptAuthentication(), databaseName, tableName, _logger);
            var kustoConnectionStringBuilder = new KustoConnectionStringBuilder(clusterUri).WithAadUserPromptAuthentication();
            using (var ingestClient = KustoIngestFactory.CreateDirectIngestClient(kustoConnectionStringBuilder))
            {
                // Sample data to be ingested
                var data = new List<Template>
                {
                    new Template
                    {
                        TimeStamp = DateTime.UtcNow,
                        Name = "Test Sample",
                        Url = "TestURL",                        
                        Author = "Liang",
                        Catalog = "Test",
                        Website = "TestURL",
                        Tags = "azd-template,azd-ai-template",
                        Star = 1,
                        Fork = 1,
                        Watch = 1,
                        Vistor = 1,
                        Clone = 1
                    },
                };

                // Convert data to JSON
                string jsonData = JsonConvert.SerializeObject(data);

                // Create an ingestion operation
                var ingestionProperties = new KustoQueuedIngestionProperties(databaseName, tableName)
                {
                    Format = DataSourceFormat.multijson,
                    IngestionMapping = new IngestionMapping()
                    {                        
                        IngestionMappingReference = MappingInfo.templateInfo_jsonMappingName
                    }
                };

                _logger.LogInformation(jsonData);

                // Ingest data
                var streamSourceOptions = new StreamSourceOptions { LeaveOpen = false };
                using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonData)))
                {
                    ingestClient.IngestFromStreamAsync(stream, ingestionProperties, streamSourceOptions).GetAwaiter().GetResult();
                }

                _logger.LogInformation("Data ingested successfully.");
            }

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }

        public void CreateJsonMappingIfNotExists(KustoConnectionStringBuilder kcsb, string databaseName, string tableName, ILogger logger)
        {
            using (var adminClient = KustoClientFactory.CreateCslAdminProvider(kcsb))
            {
                var showMappingsCommand = CslCommandGenerator.GenerateTableJsonMappingsShowCommand(tableName);
                var existingMappings = adminClient.ExecuteControlCommand<IngestionMappingShowCommandResult>(databaseName, showMappingsCommand);

                //foreach (var mapping in existingMappings)
                //{
                //    logger.LogInformation(mapping.Name);
                //}

                if (existingMappings.FirstOrDefault(m => String.Equals(m.Name, MappingInfo.templateInfo_jsonMappingName, StringComparison.Ordinal)) != null)
                {
                    logger.LogInformation("Find the mappings, need update");
                    var updateMappingCommand = CslCommandGenerator.GenerateTableMappingAlterCommand(IngestionMappingKind.Json, tableName, MappingInfo.templateInfo_jsonMappingName, MappingInfo.templateInfo_jsonMapping);
                    adminClient.ExecuteControlCommand(databaseName, updateMappingCommand);
                }
                else                
                {
                    logger.LogInformation("NotFind the mappings, need create");
                    var createMappingCommand = CslCommandGenerator.GenerateTableMappingCreateCommand(IngestionMappingKind.Json, tableName, MappingInfo.templateInfo_jsonMappingName, MappingInfo.templateInfo_jsonMapping);
                    adminClient.ExecuteControlCommand(databaseName, createMappingCommand);
                }

                // Needed to make sure the mapping and table are up-to-date
                adminClient.ExecuteControlCommand(databaseName, ".clear database cache streamingingestion schema");
            }
        }
    }
}
