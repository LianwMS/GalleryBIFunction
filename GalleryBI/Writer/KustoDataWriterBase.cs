using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Ingestion;
using Kusto.Data.Net.Client;
using Kusto.Ingest;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GalleryBI
{
    public abstract class KustoDataWriterBase<T>
    {
        protected string clusterUri { get; set; }
        protected string dbName { get; set; }
        protected string tableName { get; set; }
        protected ILogger logger { get; set; }
        protected string mappingName { get; set; }
        protected ColumnMapping[] mappings { get; set; }
        protected KustoConnectionStringBuilder builder { get; set; }

        public KustoDataWriterBase(string clusterUri, string dbName, string tableName, string mappingName, ColumnMapping[] mappings, ILogger logger)
        {
            this.clusterUri = clusterUri;
            this.dbName = dbName;
            this.tableName = tableName;
            this.builder = new KustoConnectionStringBuilder(clusterUri).WithAadSystemManagedIdentity();
            //this.builder = new KustoConnectionStringBuilder(clusterUri).WithAadUserPromptAuthentication();
            this.logger = logger;
            this.mappingName = mappingName;
            this.mappings = mappings;
            this.CreateJsonMappingIfNotExists();
        }

        public async Task WriteAsync(IEnumerable<T> data)
        {
            if (data != null && data.Count() != 0)
            {
                using (var ingestClient = KustoIngestFactory.CreateDirectIngestClient(this.builder))
                {
                    // Convert data to JSON
                    string jsonData = JsonConvert.SerializeObject(data);

                    // Create an ingestion operation
                    var ingestionProperties = new KustoQueuedIngestionProperties(this.dbName, this.tableName)
                    {
                        Format = DataSourceFormat.multijson,
                        IngestionMapping = new IngestionMapping()
                        {
                            IngestionMappingReference = this.mappingName
                        }
                    };

                    // this.logger.LogInformation(jsonData);

                    // Ingest data
                    var streamSourceOptions = new StreamSourceOptions { LeaveOpen = false };
                    using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonData)))
                    {
                        await ingestClient.IngestFromStreamAsync(stream, ingestionProperties, streamSourceOptions).ConfigureAwait(false);
                    }
                }
                logger.LogInformation($"{typeof(T)} data ingested successfully.");
            }
            else
            {
                logger.LogInformation($"Skip data ingested since null or empty.");
            }
        }

        protected void CreateJsonMappingIfNotExists()
        {
            using (var adminClient = KustoClientFactory.CreateCslAdminProvider(this.builder))
            {
                var showMappingsCommand = CslCommandGenerator.GenerateTableJsonMappingsShowCommand(this.tableName);
                var existingMappings = adminClient.ExecuteControlCommand<IngestionMappingShowCommandResult>(this.dbName, showMappingsCommand);

                if (existingMappings.FirstOrDefault(m => String.Equals(m.Name, mappingName, StringComparison.Ordinal)) != null)
                {
                    logger.LogInformation("Find the mappings, need update");
                    var updateMappingCommand = CslCommandGenerator.GenerateTableMappingAlterCommand(IngestionMappingKind.Json, this.tableName, this.mappingName, this.mappings);
                    adminClient.ExecuteControlCommand(this.dbName, updateMappingCommand);
                }
                else
                {
                    logger.LogInformation("NotFind the mappings, need create");
                    var createMappingCommand = CslCommandGenerator.GenerateTableMappingCreateCommand(IngestionMappingKind.Json, this.tableName, this.mappingName, this.mappings);
                    adminClient.ExecuteControlCommand(this.dbName, createMappingCommand);
                }

                // Needed to make sure the mapping and table are up-to-date
                adminClient.ExecuteControlCommand(this.dbName, ".clear database cache streamingingestion schema");
            }
        }
    }
}
