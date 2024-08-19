using Kusto.Cloud.Platform.Utils;
using Kusto.Data.Common;

namespace GalleryBI
{
    public class Issue
    {
        public DateTime TimeStamp { get; set; }
        public string? Id { get; set; }
        public string? Url { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public string? TemplateName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }

    public class IssueMappingInfo
    {
        private static string tableCommand = ".alter table Issues ( TimeStamp:datetime, Id:string, Url:string, Title:string, Status:string, TemplateName:string, CreatedAt:datetime, ClosedAt:datetime)";
        public static string Name = "TestJsonMapping";
        public static readonly ColumnMapping[] Mapping = new ColumnMapping[]
        {
            new ColumnMapping { ColumnName = "TimeStamp",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.TimeStamp" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Id",    Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Id" },    { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Url",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Url" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Title", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Title" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Status", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Status" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "TemplateName", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.TemplateName" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "CreatedAt", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.CreatedAt" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "ClosedAt", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.ClosedAt" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
        };
    }
}
