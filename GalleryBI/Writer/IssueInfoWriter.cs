using Kusto.Data.Common;
using Microsoft.Extensions.Logging;

namespace GalleryBI
{
    public class IssueInfoWriter : KustoDataWriterBase<Issue>
    {
        public IssueInfoWriter(string clusterUri, string dbName, string tableName, string mappingName, ColumnMapping[] mapping, ILogger logger)
            : base(clusterUri, dbName, tableName, mappingName, mapping, logger)
        {
        }            
    }
}
