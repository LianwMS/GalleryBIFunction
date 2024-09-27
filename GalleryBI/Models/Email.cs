namespace GalleryBI
{
    public class Email
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? To { get; set; }
        public string? Properties { get; set; }
    }

    public class NotificationEmailTemplate
    {
        public const string SUBJECT = "Notification Email from AI Gallery";
        public const string BODY = @"<p>Hi,</p>
<p>You're receiving this email because you&rsquo;re a CODE_OWNER or top contributor with most recent activity in [repo] that is added to <a href=""[gallery_url]"">Microsoft AI Gallery</a>.</p>
<p>The template has failed to pass validation on [issue_date] due to the following issue: [issue_link]</p>
<p>Please follow the &ldquo;[How to fix]&rdquo; links in the issue to fix the errors and contact our team [team_email], to have the template revalidated and republished.</p>
<p><em><span style=""text-decoration: underline;""><strong>Failing to fix the errors in [time_win] days, will result in removal from the collection.</strong></span></em></p>
<p><em>Please do not reply to this automated email address.</em></p>
<p>Thank you for your contribution to delivering the best quality templates to our customers!<br />The AI Gallery and Template Automation team</p>";

        public static string GetBody(string? repo, string? galleryUrl, string? issueDate, string? issueLink, string? teamEmail, string? timeFrame)
        {
            return BODY.Replace("[repo]", repo)
                .Replace("[gallery_url]", galleryUrl)
                .Replace("[issue_date]", issueDate)
                .Replace("[issue_link]", issueLink)
                .Replace("[team_email]", teamEmail)
                .Replace("[time_frame]", timeFrame);
        }
    }
}
