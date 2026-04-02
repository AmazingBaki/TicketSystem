using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketSupportSystem.Data;
using TicketSupportSystem.Data.Entities;
using TicketSupportSystem.Data.Enums;
using TicketSupportSystem.DTOs.Responses;

namespace TicketSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SupportAgent")]
    public class SupportRatingsController : ControllerBase
    {
        private readonly TicketSupportSystemContext _context;
        private readonly UserManager<User> _userManager;

        public SupportRatingsController(TicketSupportSystemContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<SupportRatingDTO>>> Get()
        {
            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = email is null ? null : await _userManager.FindByEmailAsync(email);
            if (currentUser is null)
            {
                return Forbid();
            }

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var agents = await _userManager.GetUsersInRoleAsync("SupportAgent");

            // SupportAgent sees only their row; Admin sees full list.
            if (!isAdmin)
            {
                agents = agents.Where(a => a.Id == currentUser.Id).ToList();
            }

            var agentIds = agents.Select(a => a.Id).ToList();

            var tickets = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.AssignedToId != null && agentIds.Contains(t.AssignedToId.Value))
                .Select(t => new
                {
                    t.Id,
                    AgentId = t.AssignedToId!.Value,
                    t.UserId,
                    t.CreatedAt,
                    t.ClosedAt,
                    t.Status
                })
                .ToListAsync();

            var ticketIds = tickets.Select(t => t.Id).ToList();

            var comments = await _context.Comments
                .AsNoTracking()
                .Where(c => ticketIds.Contains(c.TicketId))
                .Select(c => new { c.TicketId, c.UserId, c.CreatedAt })
                .ToListAsync();

            var ratings = new List<SupportRatingDTO>();

            foreach (var agent in agents)
            {
                var agentTickets = tickets.Where(t => t.AgentId == agent.Id).ToList();
                var assignedTotal = agentTickets.Count;
                var closedTickets = agentTickets.Where(t => t.Status == Status.Closed && t.ClosedAt != null).ToList();
                var closedTotal = closedTickets.Count;

                double? avgResolutionHours = null;
                if (closedTotal > 0)
                {
                    avgResolutionHours = closedTickets
                        .Select(t => (t.ClosedAt!.Value - t.CreatedAt).TotalHours)
                        .Average();
                }

                // First response time: first comment written by agent on that ticket after creation.
                var firstResponseMinutesList = new List<double>();
                foreach (var t in agentTickets)
                {
                    var firstAgentCommentAt = comments
                        .Where(c => c.TicketId == t.Id && c.UserId == agent.Id)
                        .OrderBy(c => c.CreatedAt)
                        .Select(c => (DateTimeOffset?)c.CreatedAt)
                        .FirstOrDefault();

                    if (firstAgentCommentAt != null)
                    {
                        var minutes = (firstAgentCommentAt.Value - t.CreatedAt).TotalMinutes;
                        if (minutes >= 0) firstResponseMinutesList.Add(minutes);
                    }
                }

                double? avgFirstResponseMinutes = null;
                if (firstResponseMinutesList.Count > 0)
                {
                    avgFirstResponseMinutes = firstResponseMinutesList.Average();
                }

                ratings.Add(new SupportRatingDTO
                {
                    AgentId = agent.Id,
                    AgentName = $"{agent.Name} {agent.Surname}".Trim(),
                    AgentEmail = agent.Email ?? string.Empty,
                    AssignedTotal = assignedTotal,
                    ClosedTotal = closedTotal,
                    AvgFirstResponseMinutes = avgFirstResponseMinutes,
                    AvgResolutionHours = avgResolutionHours
                });
            }

            // Sort: most closed, then fastest resolution.
            ratings = ratings
                .OrderByDescending(r => r.ClosedTotal)
                .ThenBy(r => r.AvgResolutionHours ?? double.MaxValue)
                .ToList();

            return Ok(ratings);
        }
    }
}

