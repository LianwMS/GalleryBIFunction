namespace GalleryBI
{
    public enum IssueCatalogs
    {
        Low,
        Moderate,
        High
    }

    public class IssueDetailsTypes
    {
        public const string README = "README.md File";
        public const string LICENSE = "LICENSE.md File";
        public const string SECURITY = "SECURITY.md FILE";
        public const string CODE_OF_CONDUCT = "CODE_OF_CONDUCT.md File";
        public const string CONTRIBUTING = "CONTRIBUTING.md File";
        public const string ISSUE_TEMPLATE = "ISSUE_TEMPLATE.md File";
        public const string TOPICS = "Topics";
        public const string AZURE_DEV = "azure-dev.yml File";
        public const string AZURE_YAML = "azure.yml File";
        public const string INFRA_FOLDER = "infra Folder";
        public const string DEVCONTAINER_FOLDER = ".devcontainer Folder";
        public const string AZD_UP = "azd up";
        public const string AZD_DOWN = "azd down";
        public const string SECURITY_SCAN = "Security Scan";

        public static readonly List<string> All = new List<string>
        {
            README,
            LICENSE,
            SECURITY,
            CODE_OF_CONDUCT,
            CONTRIBUTING,
            ISSUE_TEMPLATE,
            TOPICS,
            AZURE_DEV,
            AZURE_YAML,
            INFRA_FOLDER,
            DEVCONTAINER_FOLDER,
            AZD_UP,
            AZD_DOWN,
            SECURITY_SCAN
        };
    } 
}
