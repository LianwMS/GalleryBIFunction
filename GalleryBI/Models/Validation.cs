using Kusto.Cloud.Platform.Utils;
using Kusto.Data.Common;

namespace GalleryBI
{
    public class Validation
    {
        public DateTime TimeStamp { get; set; }
        public string? Url { get; set; }
        public DateTime RunTime { get; set; }
        public string? Result { get; set; }
        public string? Details { get; set; }
        public string? WorkflowRepoName { get; set; }
        public string? WorkflowName { get; set; }
        public string? RunId { get; set; }
        public string? JobId { get; set; }
        public DateTime RunStartTime { get; set; }
        public string? AttemptId { get; set; }
    }

    public class ValidationMappingInfo
    {
        private static string tableCommand = ".alter table Validations ( TimeStamp:datetime, Url:string, RunTime:datetime, Result:string, Details:string, WorkflowRepoName: string, WorkflowName: string, RunId: string, JobId: string, RunStartTime: datetime, AttemptId: string)";
        public static string Name = "ValidationMapping";
        public static readonly ColumnMapping[] Mapping = new ColumnMapping[]
        {
            new ColumnMapping { ColumnName = "TimeStamp",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.TimeStamp" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Url", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Url" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "RunTime", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.RunTime" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Result", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Result" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Details", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Details" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "WorkflowRepoName", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.WorkflowRepoName" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "WorkflowName", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.WorkflowName" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "RunId", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.RunId" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "JobId", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.JobId" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "RunStartTime", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.RunStartTime" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "AttemptId", Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.AttemptId" }, { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
        };
    }
}
