using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Microsoft.Extensions.Logging;

namespace GalleryBI
{
    public class EmailInfoWriter : KustoDataWriterBase<Email>
    {
        public EmailInfoWriter(string clusterUri, string dbName, string tableName, string mappingName, ColumnMapping[] mapping, ILogger logger)
            : base(clusterUri, dbName, tableName, mappingName, mapping, logger)
        {
        }

        public async Task<IEnumerable<Email>> RemoveDup(IEnumerable<Email> data)
        {
            var existList = new List<EmailMetadata>();
            var result = new List<Email>();
            // Query from Kusto table with data > -10 day
            using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(this.builder))
            {
                var query = $"{this.tableName} | where TimeStamp > ago(14d)";
                var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };
                var response = await queryProvider.ExecuteQueryAsync(this.dbName, query, clientRequestProperties).ConfigureAwait(false);
                int colIssueLink = response.GetOrdinal("IssueLink");
                int colEmailCatalog = response.GetOrdinal("EmailCatalog");

                while (response.Read())
                {
                    int catalog = 0;
                    Int32.TryParse(response.GetString(colEmailCatalog), out catalog);
                    var existEmailMetadata = new EmailMetadata()
                    {
                        IssueLink = response.GetString(colIssueLink),
                        EmailTemplateCatalogs = (EmailTemplateCatalogs)catalog
                    };
                    existList.Add(existEmailMetadata);
                }
            }

            using (IEnumerator<Email> enumerator = data.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var email = enumerator.Current;
                    if (existList.Where(m => m.IssueLink == email.Metadata.IssueLink && m.EmailTemplateCatalogs == email.Metadata.EmailTemplateCatalogs).Count() == 0)
                    {
                        result.Add(email);
                    }
                }
            }

            return result;
        }
    }
}
