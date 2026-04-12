using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSupportSystem.Data;
using TicketSupportSystem.Data.Entities;

namespace TicketSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly TicketSupportSystemContext _context;

        public RolesController(RoleManager<Role> roleManager, UserManager<User> userManager, TicketSupportSystemContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new Role { Name = roleName });

                if (roleResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(roleResult);
                }
            }

            return Conflict();
        }


        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (user != null && roleExist)
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result);
                }
            }

            return NotFound();
        }

        [HttpPost("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (user != null && roleExist)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result);
                }
            }

            return NotFound();
        }
        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }

            // Снять назначение с тикетов, где этот пользователь — исполнитель
            var assignedTickets = await _context.Tickets
                .Where(t => t.AssignedToId == user.Id)
                .ToListAsync();
            foreach (var ticket in assignedTickets)
            {
                ticket.AssignedToId = null;
            }
            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Не удалось удалить пользователя.", errors = result.Errors.Select(e => e.Description) });
            }

            return Ok();
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var result = new List<object>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new
                {
                    id = u.Id,
                    email = u.Email ?? string.Empty,
                    name = $"{u.Name} {u.Surname}".Trim(),
                    roles
                });
            }
            return Ok(result);
        }

        [HttpGet("GetAgents")]
        [Authorize(Roles = "Admin,SupportAgent")]
        public async Task<IActionResult> GetAgents()
        {
            var agents = await _userManager.GetUsersInRoleAsync("SupportAgent");
            var result = agents
                .OrderBy(a => a.Surname)
                .Select(a => new
                {
                    id = a.Id,
                    name = $"{a.Name} {a.Surname}".Trim(),
                    email = a.Email ?? string.Empty
                });
            return Ok(result);
        }
    }
}
