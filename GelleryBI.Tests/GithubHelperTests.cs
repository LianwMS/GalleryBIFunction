using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Octokit;

namespace GalleryBI.Tests
{
    [TestClass]
    public class GithubHelperTests
    {
        private readonly GitHubClient githubClient;
        private ILogger logger;

        public GithubHelperTests()
        {
            logger = new TestConsoleLogge();
            var accessToken = AzureHelper.GetSecretFromKV(TestAppContext.BIKVUri, TestAppContext.AzurePATSecretName).Result;
            this.githubClient = new GitHubClient(new Octokit.ProductHeaderValue("GithubHelperTests"));
            githubClient.Credentials = new Credentials(accessToken);
        }

        [TestMethod]
        public void TestGetTemplateOwner()
        {
            var reader = new TemplateInfoReader(logger);
            var result = reader.ReadAsync().Result;
            var emailDic = new Dictionary<string, string>();

            for (int i = 0; i < result.Count(); i++)
            {
                var template = result.ElementAt(i);
                var owners = GithubHelper.GetTemplateOwner(githubClient, template).Result;
                emailDic.Add(template.Url, owners);
            }
        }

        [TestMethod]
        public void TestGetIssueCatalog()
        {
            var issueUrl = "https://github.com/microsoft/template-validation-action/issues/46";
            var catalog = GithubHelper.GetIssueCatalog(githubClient, issueUrl).Result;
            Assert.AreEqual(IssueCatalogs.High, catalog);
        }

        [TestMethod]
        public void TestGetIssueOpenDate()
        {
            var issueUrl = "https://github.com/microsoft/template-validation-action/issues/46";
            var dateTime = GithubHelper.GetIssueOpenDateTime(githubClient, issueUrl).Result;
            Assert.AreNotEqual(DateTime.MinValue, dateTime);

            var dateTime2 = GithubHelper.GetIssueOpenDateTime(githubClient, "https://github.com/Azure-Samples/agent-python-openai-prompty-langchain/issues/30").Result;
            Assert.AreNotEqual(DateTime.MinValue, dateTime2);
        }
    }
}
