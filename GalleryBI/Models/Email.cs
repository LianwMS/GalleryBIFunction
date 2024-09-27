using Kusto.Cloud.Platform.Utils;
using Kusto.Data.Common;

namespace GalleryBI
{
    public class Email
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? To { get; set; }
        public string? Cc { get; set; }
        public EmailMetadata Metadata { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class EmailMetadata
    {
        public string Repo { get; set; }
        public string IssueDate { get; set; }
        public string IssueLink { get; set; }
        public IssueCatalogs IssueCatalog { get; set; }
        public string OwnerEmailList { get; set; }
        public EmailTemplateCatalogs EmailTemplateCatalogs { get; set; }
    }

    public class EmailMappingInfo
    {
        private static string tableCommand = ".alter table Emails ( TimeStamp:datetime, Subject:string, To:string, Cc:string, Repo:string, IssueLink:string, IssueCatalog:string, EmailCatalog:string)";
        public static string Name = "EmailMapping";
        public static readonly ColumnMapping[] Mapping = new ColumnMapping[]
        {
            new ColumnMapping { ColumnName = "TimeStamp",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.TimeStamp" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Subject",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Subject" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "To",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.To" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Cc",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Cc" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "Repo",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Metadata.Repo" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "IssueLink",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Metadata.IssueLink" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "IssueCatalog",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Metadata.IssueCatalog" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
            new ColumnMapping { ColumnName = "EmailCatalog",  Properties = new Dictionary<string, string>{ { MappingConsts.Path, "$.Metadata.EmailTemplateCatalogs" },  { MappingConsts.TransformationMethod, CsvFromJsonStream_TransformationMethod.None.FastToString() } } },
        };
    }

    public static class EmailMetadataExtension
    {
        public static Dictionary<EmailTemplateCatalogs, EmailTemplate> TemplateCollection = new Dictionary<EmailTemplateCatalogs, EmailTemplate>()
        {
            { EmailTemplateCatalogs.FirstFixNotification, new FistFixNotificationTemplate() },
            { EmailTemplateCatalogs.LastFixNotification, new LastFixNotificationTemplate()}
        };

        public static Email GenerateEmail(this EmailMetadata metadata)
        {
            return new Email
            {
                Subject = TemplateCollection[metadata.EmailTemplateCatalogs].GetSubject(metadata),
                Body = TemplateCollection[metadata.EmailTemplateCatalogs].GetBody(metadata),
                To = metadata.OwnerEmailList,
                Cc = AppContext.EmailCc,
                Metadata = metadata,
                TimeStamp = DateTime.UtcNow,
            };
        }
    }


}
