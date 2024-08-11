using Kusto.Cloud.Platform.Utils;
using Kusto.Data.Common;

namespace GalleryBI
{
    internal class Template
    {
        public DateTime TimeStamp { get; set; }
        public string? Catalog { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Author { get; set; }
        public string? Website { get; set; }
        public string? Tags { get; set; }
        public long Star { get; set; }
        public long Fork { get; set; }
        public long Watch { get; set; }
        public long Vistor { get; set; }
        public long Clone { get; set; }
    }
    internal class MappingInfo
    {
        public static string templateInfo_jsonMappingName = "TestJsonMapping";
        public static readonly ColumnMapping[] templateInfo_jsonMapping = new ColumnMapping[]
        {
            new ColumnMapping { ColumnName = "TimeStamp",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.TimeStamp" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Catalog",    Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Catalog" },    { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Name",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Name" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Url", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Url" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Author", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Author" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Website", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Website" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Tags", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Tags" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Star", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Star" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Fork", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Fork" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Watch", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Watch" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Vistor", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Vistor" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Clone", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Clone" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
        };
    }
}
