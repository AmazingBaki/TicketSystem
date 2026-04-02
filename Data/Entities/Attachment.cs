namespace TicketSupportSystem.Data.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public string StoredFileName { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
        public string Path { get; set; } = null!;

        public Guid? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public Guid? TicketId { get; set; }
        public Ticket? Ticket { get; set; }

    }
}
