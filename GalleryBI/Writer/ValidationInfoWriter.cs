using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Microsoft.Extensions.Logging;

namespace GalleryBI
{
    public class ValidationInfoWriter : KustoDataWriterBase<Validation>
    {
        public ValidationInfoWriter(string clusterUri, string dbName, string tableName, string mappingName, ColumnMapping[] mapping, ILogger logger)
           : base(clusterUri, dbName, tableName, mappingName, mapping, logger)
        {
        }

        public async Task<IEnumerable<Validation>> RemoveDup(IEnumerable<Validation> data)
        {
            var existList = new List<Validation>();
            var result = new List<Validation>();
            // Query from Kusto table with data > -10 day
            using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(this.builder))
            {
                var query = $"{this.tableName} | where TimeStamp > ago(8d)";
                var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };
                var response = await queryProvider.ExecuteQueryAsync(this.dbName, query, clientRequestProperties).ConfigureAwait(false);
                int colRunId = response.GetOrdinal("RunId");
                int colJobId = response.GetOrdinal("JobId");
                int colUrl = response.GetOrdinal("Url");

                while (response.Read())
                {
                    var existValidation = new Validation()
                    {
                        RunId = response.GetString(colRunId),
                        JobId = response.GetString(colJobId),
                        Url = response.GetString(colUrl),
                    };
                    existList.Add(existValidation);
                }
            }

            using (IEnumerator<Validation> enumerator = data.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var validation = enumerator.Current;
                    if (existList.Where(e => e.RunId == validation.RunId && e.JobId == validation.JobId && e.Url == validation.Url).Count() == 0)
                    {
                        result.Add(validation);
                    }
                }
            }

            return result;
        }
    }
}
