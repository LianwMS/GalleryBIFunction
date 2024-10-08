﻿using Kusto.Cloud.Platform.Utils;
using Kusto.Data.Common;
using Newtonsoft.Json;

namespace GalleryBI
{
    public class Template
    {
        public DateTime TimeStamp { get; set; }
        public string? Catalog { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        [JsonIgnore]
        public string? Author { get; set; }
        public string? Website { get; set; }
        public List<string>? Tags { get; set; }
        public long Star { get; set; }
        public long Fork { get; set; }
        public long Watch { get; set; }
        public long Vistor { get; set; }
        public long Clone { get; set; }
        public List<string>? Topics { get; set; }
        public string? Source { get; set; }
        public List<string>? ValidationActiveIssues { get; set; }
        public List<string>? ValidationNonActiveIssues { get; set; }
    }
    public class TemplateMappingInfo
    {
        private static string tableCommand = ".alter table TestTemplateInfo ( TimeStamp:datetime, Url:string, Name:string, Catalog:string, Author:string, Website:string, Tags:string, Star:long, Fork:long, Watch:long, Vistor:long, Clone:long, Topics:string, ValidationActiveIssues:string, ValidationNonActiveIssues:string)";
        public static string Name = "TestJsonMapping";
        public static readonly ColumnMapping[] Mapping = new ColumnMapping[]
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
            new ColumnMapping { ColumnName = "Topics", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Topics" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "ValidationActiveIssues", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.ValidationActiveIssues" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "ValidationNonActiveIssues", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.ValidationNonActiveIssues" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
        };
    }
}
