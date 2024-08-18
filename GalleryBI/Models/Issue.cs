namespace GalleryBI
{
    public class Issue
    {
        public DateTime TimeStamp { get; set; }
        public string? Id { get; set; }
        public string? Url { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public string? TemplateName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}
