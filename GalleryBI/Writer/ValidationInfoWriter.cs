using Kusto.Data.Common;
using Kusto.Ingest;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GalleryBI
{
    public class ValidationInfoWriter : KustoDataWriterBase<Validation>
    {
        public ValidationInfoWriter(string clusterUri, string dbName, string tableName, string mappingName, ColumnMapping[] mapping, ILogger logger)
           : base(clusterUri, dbName, tableName, mappingName, mapping, logger)
        {
        }

        public override async Task WriteAsync(IEnumerable<Validation> data)
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
        }
    }
}
