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
        public const string ValidationRepoOwner = "Azure";
        public const string ValidationRepoName = "ai-apps";
        public const string ValidationWorkflowFileName = "regular-validate.yaml";        

        // BI Cluster Info
        public const string ClusterUri = "https://gallerybicluster.eastus.kusto.windows.net";
        public const string BIDBName = "BIData";
        public const string TemplateInfoTableName = "TemplateInfo";
        public const string ValidationInforTableName = "Validations";
        public const string IssueInfoTableName = "Issues";
        public const string EmailTableName = "Emails";

        // Email
        public const string EmailTriggerUrl = "https://prod-22.eastus.logic.azure.com:443/workflows/c19d0ee9dd904a6e9f873721381cb8bd/triggers/When_a_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_a_HTTP_request_is_received%2Frun&sv=1.0&sig=irsB2pcv2w8R25dLqDG3j2TGyY6pd5nQOGdeZeHZ_9s";
        public const string TemplateDefaultOwner = "nvenditto@microsoft.com";
        public const string EmailCc = "nvenditto@microsoft.com;lianw@microsoft.com";

        //Unpublish
        public const string UnpublishWFRepoOwner = "Azure";
        public const string UnpublishWFRepoName = "ai-apps";
        public const string UnpublishWFName = "unpublish.yaml";
    }
}
