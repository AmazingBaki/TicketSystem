using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketSupportSystem.Common.Exceptions;
using TicketSupportSystem.Data;
using TicketSupportSystem.Data.Entities;
using TicketSupportSystem.DTOs.Responses;
using TicketSupportSystem.Interfaces;

namespace TicketSupportSystem.Services
{
    public class FileService : IFileService
    {
        private readonly string _attachmentsDir;
        private readonly TicketSupportSystemContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public FileService(IWebHostEnvironment env, TicketSupportSystemContext context, IMapper mapper)
        {
            _env = env;
            _attachmentsDir = "attachments";
            _context = context;
            _mapper = mapper;
        }

        private string EnsureAttachmentsDir()
        {
            var dir = Path.Combine(_env.WebRootPath, _attachmentsDir);
            Directory.CreateDirectory(dir);
            return dir;
        }

        private async Task<Attachment> SaveToDisk(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File not selected");
            }

            EnsureAttachmentsDir();

            var storedFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(_env.WebRootPath, _attachmentsDir, storedFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var filePath = Path.Combine(_attachmentsDir, storedFileName);

            return new Attachment
            {
                StoredFileName = storedFileName,
                OriginalFileName = file.FileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Size = file.Length,
                Path = filePath,
            };
        }

        public async Task<AttachmentDTO> SaveCommentAttachment(IFormFile file, Guid commentId)
        {
            var attachment = await SaveToDisk(file);
            attachment.CommentId = commentId;

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            var attachmentDTO = _mapper.Map<Attachment, AttachmentDTO>(attachment);

            return attachmentDTO;
        }

        public async Task<AttachmentDTO> SaveTicketAttachment(IFormFile file, Guid ticketId)
        {
            var attachment = await SaveToDisk(file);
            attachment.TicketId = ticketId;

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            return _mapper.Map<Attachment, AttachmentDTO>(attachment);
        }

        public async Task<List<AttachmentDTO>> GetAttachmentsToComment(Guid commentId)
        {
            var comment = await _context.Comments
                .Include(c => c.Attachments)
                .SingleOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
            {
                throw new NotFoundException();
            }

            var attachments = comment.Attachments.ToList();

            var attachmentsDTOs = _mapper.Map<List<Attachment>, List<AttachmentDTO>>(attachments);

            return attachmentsDTOs;
        }

        public async Task<List<AttachmentDTO>> GetAttachmentsToTicket(Guid ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Attachments)
                .SingleOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                throw new NotFoundException();
            }

            var attachmentsDTOs = _mapper.Map<List<Attachment>, List<AttachmentDTO>>(ticket.Attachments.ToList());
            return attachmentsDTOs;
        }

        public async Task DeleteAttachment(Guid id)
        {
            var attachment = await _context.Attachments
                .FirstOrDefaultAsync(f => f.Id == id);

            if (attachment == null)
            {
                throw new NotFoundException("File not found");
            }

            var fullPath = Path.Combine(_env.WebRootPath, attachment.Path);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task<(Stream Stream, string ContentType, string DownloadName)> OpenAttachment(Guid id)
        {
            var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
            if (attachment == null)
            {
                throw new NotFoundException("File not found");
            }

            var fullPath = Path.Combine(_env.WebRootPath, attachment.Path);
            if (!File.Exists(fullPath))
            {
                throw new NotFoundException("File not found");
            }

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (stream, attachment.ContentType, attachment.OriginalFileName);
        }
    }
}
