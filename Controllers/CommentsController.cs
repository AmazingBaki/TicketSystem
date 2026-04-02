using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TicketSupportSystem.Data;
using TicketSupportSystem.Common.Exceptions;
using TicketSupportSystem.Data.Entities;
using TicketSupportSystem.DTOs.Requests;
using TicketSupportSystem.DTOs.Responses;
using TicketSupportSystem.Interfaces;
using TicketSupportSystem.Services;
using TicketSupportSystem.Validators;

namespace TicketSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;
        private readonly UserManager<User> _userManager;
        private readonly TicketSupportSystemContext _context;
        private IValidator<UpdateCommentDTO> _updateCommentValidator;

        public CommentsController(ICommentsService commentsService, UserManager<User> userManager, TicketSupportSystemContext context, IValidator<UpdateCommentDTO> updateCommentValidator)
        {
            _commentsService = commentsService;
            _userManager = userManager;
            _context = context;
            _updateCommentValidator = updateCommentValidator;
        }

        [HttpPut("UpdateComment/{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, UpdateCommentDTO commentDTO)
        {
            var validationRes = _updateCommentValidator.Validate(commentDTO);
            if (!validationRes.IsValid)
                return BadRequest(validationRes);

            try
            {
                var currentUserEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrWhiteSpace(currentUserEmail))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByEmailAsync(currentUserEmail);
                if (user is null)
                {
                    return NotFound();
                }

                var comment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                if (comment is null)
                {
                    return NotFound();
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin") || roles.Contains("SupportAgent"))
                {
                    await _commentsService.UpdateComment(id, commentDTO);
                    return Ok();
                }

                if (roles.Contains("Customer") && comment.UserId == user.Id)
                {
                    await _commentsService.UpdateComment(id, commentDTO);
                    return Ok();
                }

                return Forbid();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("DeleteComment/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteComment(Guid id)
        {
            try
            {
                await _commentsService.DeleteComment(id);

                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
