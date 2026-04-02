using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TicketSupportSystem.Data.Entities;
using TicketSupportSystem.DTOs.Requests;
using TicketSupportSystem.DTOs.Responses;

namespace TicketSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        private async Task<User?> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _userManager.FindByEmailAsync(email);
        }

        [HttpGet]
        public async Task<ActionResult<CurrentUserDTO>> Get()
        {
            var user = await GetCurrentUser();
            if (user is null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new CurrentUserDTO
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProfileDTO dto)
        {
            var user = await GetCurrentUser();
            if (user is null) return Unauthorized();

            user.Name = dto.Name?.Trim() ?? "";
            user.Surname = dto.Surname?.Trim() ?? "";

            // Identity fields
            if (!string.Equals(user.UserName, dto.UserName, StringComparison.Ordinal))
            {
                var setUserName = await _userManager.SetUserNameAsync(user, dto.UserName);
                if (!setUserName.Succeeded) return BadRequest(setUserName);
            }

            if (!string.Equals(user.PhoneNumber, dto.PhoneNumber, StringComparison.Ordinal))
            {
                var setPhone = await _userManager.SetPhoneNumberAsync(user, dto.PhoneNumber);
                if (!setPhone.Succeeded) return BadRequest(setPhone);
            }

            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded) return BadRequest(update);

            return NoContent();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            var user = await GetCurrentUser();
            if (user is null) return Unauthorized();

            var res = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!res.Succeeded) return BadRequest(res);
            return NoContent();
        }

        [HttpPost("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDTO dto)
        {
            var user = await GetCurrentUser();
            if (user is null) return Unauthorized();

            // Verify password for sensitive action
            if (!await _userManager.CheckPasswordAsync(user, dto.CurrentPassword))
            {
                return Unauthorized();
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.NewEmail);
            var res = await _userManager.ChangeEmailAsync(user, dto.NewEmail, token);
            if (!res.Succeeded) return BadRequest(res);

            // Keep username in sync if it equals old email or empty
            if (string.IsNullOrWhiteSpace(user.UserName) || string.Equals(user.UserName, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.SetUserNameAsync(user, dto.NewEmail);
            }

            return NoContent();
        }
    }
}

