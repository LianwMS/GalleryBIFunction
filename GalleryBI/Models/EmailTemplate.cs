namespace GalleryBI
{
    public enum EmailTemplateCatalogs
    {
        FirstFixNotification,
        LastFixNotification
    }

    public interface EmailTemplate
    {
        public string GetSubject(EmailMetadata metadata);


        public string GetBody(EmailMetadata metadata);

    }

    public class FistFixNotificationTemplate : EmailTemplate
    {
        public const string SUBJECT = "[Action Required][template_name] in AI Collection validation failed";
        public const string BODY = @"<p>Hi,</p>
<p>You're receiving this email because you&rsquo;re a CODE_OWNER or top contributor with most recent activity in [repo] that is added to <a href=""https://aka.ms/aiapps"">Microsoft AI Gallery</a>.</p>
<h3>Issue Found</h3>
<p>The template has failed to pass validation on [issue_date] due to the following issue: [issue_link]</p>
<h3>Action Required</h3>
<p>Please follow the &ldquo;[How to fix]&rdquo; links in the issue to fix the errors and contact our <a href=""https://github.com/orgs/Azure-Samples/teams/template-automation-admins"">team</a>, to have the template revalidated and republished.</p>
<p><em><span style=""text-decoration: underline;""><strong>Failing to fix the errors in [time_win] days, will result in removal from the collection.</strong></span></em></p>
<p><em>Please do not reply to this automated email address.</em></p>
<p>Thank you for your contribution to delivering the best quality templates to our customers!<br />The AI Gallery and Template Automation team</p>";

        public string GetSubject(EmailMetadata emailMetadata)
        {
            var (owner, repoName) = GithubHelper.GetRepoOwnerAndName(emailMetadata.Repo);
            return SUBJECT.Replace("[template_name]", repoName);
        }

        public string GetBody(EmailMetadata metadata)
        {
            if (metadata.IssueCatalog == IssueCatalogs.High)
            {
                return BODY.Replace("[repo]", metadata.Repo)
                   .Replace("[issue_date]", metadata.IssueDate)
                   .Replace("[issue_link]", metadata.IssueLink)
                   .Replace("[time_win]", "1 days");
            }
            else
            {
                return BODY.Replace("[repo]", metadata.Repo)
                   .Replace("[issue_date]", metadata.IssueDate)
                   .Replace("[issue_link]", metadata.IssueLink)
                   .Replace("[time_win]", "7 days");
            }
        }
    }

    public class LastFixNotificationTemplate : EmailTemplate
    {
        public const string SUBJECT = "[Action Required][template_name] in AI Collection validation failed";
        public const string BODY = @"<p>Hi,</p>
<p>You're receiving this email because you&rsquo;re a CODE_OWNER or top contributor with most recent activity in [repo] that is added to <a href=""https://aka.ms/aiapps"">Microsoft AI Gallery</a>.</p>
<h3>Issue Found</h3>
<p>The template has failed to pass validation on [issue_date] due to the following issue: [issue_link]</p>
<h3>Action Required</h3>
<p>Please follow the &ldquo;[How to fix]&rdquo; links in the issue to fix the errors and contact our <a href=""https://github.com/orgs/Azure-Samples/teams/template-automation-admins"">team</a>, to have the template revalidated and republished.</p>
<p><em><span style=""text-decoration: underline;""><strong>This is the last notification before removal from the collection, in 48 hours.</strong></span></em></p>
<p><em>Please do not reply to this automated email address.</em></p>
<p>Thank you for your contribution to delivering the best quality templates to our customers!<br />The AI Gallery and Template Automation team</p>";

        public string GetSubject(EmailMetadata emailMetadata)
        {
            var (owner, repoName) = GithubHelper.GetRepoOwnerAndName(emailMetadata.Repo);
            return SUBJECT.Replace("[template_name]", repoName);
        }

        public string GetBody(EmailMetadata metadata)
        {
            return BODY.Replace("[repo]", metadata.Repo)
               .Replace("[issue_date]", metadata.IssueDate)
               .Replace("[issue_link]", metadata.IssueLink);
        }
    }
}

