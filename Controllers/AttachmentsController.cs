using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TicketSupportSystem.Common.Exceptions;
using TicketSupportSystem.Data;
using TicketSupportSystem.Data.Entities;
using TicketSupportSystem.Interfaces;

namespace TicketSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly TicketSupportSystemContext _context;
        private readonly UserManager<User> _userManager;

        public AttachmentsController(IFileService fileService, TicketSupportSystemContext context, UserManager<User> userManager)
        {
            _fileService = fileService;
            _context = context;
            _userManager = userManager;
        }

        private async Task<(User User, bool IsAdminOrSupport)> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var user = string.IsNullOrWhiteSpace(email) ? null : await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new ForbiddenException();
            }
            var isAdminOrSupport = await _userManager.IsInRoleAsync(user, "Admin")
                || await _userManager.IsInRoleAsync(user, "SupportAgent");
            return (user, isAdminOrSupport);
        }

        private async Task<bool> CanAccessTicket(Guid ticketId)
        {
            var (user, isAdminOrSupport) = await GetCurrentUser();
            if (isAdminOrSupport) return true;
            return await _context.Tickets.AnyAsync(t => t.Id == ticketId && t.UserId == user.Id);
        }

        [HttpPost("Comments/UploadAttachment/{commentId}")]
        public async Task<IActionResult> UploadAttachment(Guid commentId, IFormFile file)
        {
            try
            {
                var comment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == commentId);
                if (comment is null) return NotFound();
                if (!await CanAccessTicket(comment.TicketId)) return Forbid();

                var attachment = await _fileService.SaveCommentAttachment(file, commentId);
                return Ok(attachment);
            } 
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Comments/GetAttachmentsToComment/{commentId}")]
        public async Task<IActionResult> GetAttachmentsToComment(Guid commentId)
        {
            try
            {
                var comment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == commentId);
                if (comment is null) return NotFound();
                if (!await CanAccessTicket(comment.TicketId)) return Forbid();

                var attachments = await _fileService.GetAttachmentsToComment(commentId);
                return Ok(attachments);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("Comments/DeleteAttachment/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(Guid attachmentId)
        {
            try
            {
                var attachment = await _context.Attachments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == attachmentId);
                if (attachment is null) return NotFound();
                var ticketId = attachment.TicketId;
                if (attachment.CommentId is not null)
                {
                    var comment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == attachment.CommentId);
                    if (comment is null) return NotFound();
                    ticketId = comment.TicketId;
                }
                if (ticketId is null) return Forbid();
                if (!await CanAccessTicket(ticketId.Value)) return Forbid();

                await _fileService.DeleteAttachment(attachmentId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("Tickets/UploadAttachment/{ticketId}")]
        public async Task<IActionResult> UploadTicketAttachment(Guid ticketId, IFormFile file)
        {
            if (!await CanAccessTicket(ticketId)) return Forbid();
            try
            {
                var exists = await _context.Tickets.AsNoTracking().AnyAsync(t => t.Id == ticketId);
                if (!exists) return NotFound();

                var attachment = await _fileService.SaveTicketAttachment(file, ticketId);
                return Ok(attachment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Tickets/GetAttachmentsToTicket/{ticketId}")]
        public async Task<IActionResult> GetAttachmentsToTicket(Guid ticketId)
        {
            if (!await CanAccessTicket(ticketId)) return Forbid();
            try
            {
                var attachments = await _fileService.GetAttachmentsToTicket(ticketId);
                return Ok(attachments);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Download/{attachmentId}")]
        public async Task<IActionResult> Download(Guid attachmentId)
        {
            var attachment = await _context.Attachments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == attachmentId);
            if (attachment is null) return NotFound();

            Guid? ticketId = attachment.TicketId;
            if (ticketId is null && attachment.CommentId is not null)
            {
                var comment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == attachment.CommentId);
                ticketId = comment?.TicketId;
            }
            if (ticketId is null) return Forbid();
            if (!await CanAccessTicket(ticketId.Value)) return Forbid();

            try
            {
                var (stream, contentType, downloadName) = await _fileService.OpenAttachment(attachmentId);
                return File(stream, contentType, downloadName);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
