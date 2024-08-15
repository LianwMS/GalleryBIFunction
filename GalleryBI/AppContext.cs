﻿namespace GalleryBI
{
    internal class AppContext
    {
        // KV PAT Info
        public const string BIKVName = "gallerybiakv";
        public const string BIKVUri = $"https://{BIKVName}.vault.azure.net/";
        public const string PATSecretName = "finegrainedpat";

        // Template Info
        public const string TemplateFileUri = "https://api.github.com/repos/Azure/ai-apps/contents/website/static/templates.json";        
        public const string GithubRepoOwner = "Azure";
        public const string GithubRepoName = "ai-apps";
        public const string TemplateFilePath = "website/static/templates.json";

        // BI Cluster Info
        public const string ClusterUri = "https://gallerybicluster.eastus.kusto.windows.net";
        public const string BIDBName = "BIData";
        public const string TemplateInfoTableName = "TestTemplateInfo";

    }
}
