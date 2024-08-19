namespace GalleryBI
{
    internal class AppContext
    {
        // KV PAT Info
        public const string BIKVName = "gallerybiakv";
        public const string BIKVUri = $"https://{BIKVName}.vault.azure.net/";
        public const string AzurePATSecretName = "finegrainedpat";
        public const string LianwPATSecretName = "lianwgithubpat";
        public const string Hund030PATSecretName = "hund030githubpat";

        // Template Info
        //public const string TemplateFileUri = "https://api.github.com/repos/Azure/ai-apps/contents/website/static/templates.json";
        public const string GithubRepoOwner = "Azure";
        public const string GithubRepoName = "ai-apps";
        public const string TemplateFilePath = "website/static/templates.json";

        // Validation
        public const string ValidationRepoOwner = "hund030";
        public const string ValidationRepoName = "ai-apps";
        public const string ValidationWorkflowName = "Regular Template Validation";

        // BI Cluster Info
        public const string ClusterUri = "https://gallerybicluster.eastus.kusto.windows.net";
        public const string BIDBName = "BIData";
        public const string TemplateInfoTableName = "TemplateInfo";
        public const string ValidationInforTableName = "Validations";
        public const string IssueInfoTableName = "Issues";
    }
}
