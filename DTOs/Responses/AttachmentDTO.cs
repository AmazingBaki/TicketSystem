namespace TicketSupportSystem.DTOs.Responses
{
    public class AttachmentDTO
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
    }
}
