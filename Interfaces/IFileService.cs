using System.Net.Mail;
using TicketSupportSystem.DTOs.Responses;

namespace TicketSupportSystem.Interfaces
{
    public interface IFileService
    {
        public Task<AttachmentDTO> SaveCommentAttachment(IFormFile file, Guid commentId);
        public Task<AttachmentDTO> SaveTicketAttachment(IFormFile file, Guid ticketId);
        public Task<List<AttachmentDTO>> GetAttachmentsToComment(Guid commentId);
        public Task<List<AttachmentDTO>> GetAttachmentsToTicket(Guid ticketId);
        public Task DeleteAttachment(Guid id);
        public Task<(Stream Stream, string ContentType, string DownloadName)> OpenAttachment(Guid id);
    }
}
