using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TicketSupportSystem.Data;
using TicketSupportSystem.Data.Entities;
using TicketSupportSystem.Hubs;
using TicketSupportSystem.Interfaces;

namespace TicketSupportSystem.Services;

public class TicketNotificationService : ITicketNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly UserManager<User> _userManager;
    private readonly TicketSupportSystemContext _db;

    public TicketNotificationService(
        IHubContext<NotificationHub> hubContext,
        UserManager<User> userManager,
        TicketSupportSystemContext db)
    {
        _hubContext = hubContext;
        _userManager = userManager;
        _db = db;
    }

    public async Task NotifyNewTicketAsync(Guid ticketId, Guid creatorUserId, string title)
    {
        var clientPayload = new
        {
            kind = "newTicketClient",
            ticketId = ticketId.ToString(),
            title
        };
        await SendToUserByIdAsync(creatorUserId, clientPayload);

        var staffPayload = new
        {
            kind = "newTicketStaff",
            ticketId = ticketId.ToString(),
            title
        };
        await SendToAllStaffAsync(staffPayload, exceptUserId: creatorUserId);
    }

    public async Task NotifyNewCommentAsync(Guid ticketId, Guid commentAuthorUserId)
    {
        var ticket = await _db.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null)
        {
            return;
        }

        var author = await _userManager.FindByIdAsync(commentAuthorUserId.ToString());
        if (author is null)
        {
            return;
        }

        var roles = await _userManager.GetRolesAsync(author);
        var payload = new
        {
            kind = "ticketReply",
            ticketId = ticketId.ToString(),
            title = ticket.Title
        };

        if (roles.Contains("Customer"))
        {
            await SendToAllStaffAsync(payload, exceptUserId: commentAuthorUserId);
            return;
        }

        if (roles.Contains("Admin") || roles.Contains("SupportAgent"))
        {
            if (ticket.UserId.HasValue && ticket.UserId.Value != commentAuthorUserId)
            {
                await SendToUserByIdAsync(ticket.UserId.Value, payload);
            }
        }
    }

    private async Task SendToUserByIdAsync(Guid userId, object payload)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user?.Email is null)
        {
            return;
        }

        await _hubContext.Clients
            .Group(NotificationHub.GroupNameForEmail(user.Email))
            .SendAsync("notify", payload);
    }

    private async Task SendToAllStaffAsync(object payload, Guid? exceptUserId)
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        var agents = await _userManager.GetUsersInRoleAsync("SupportAgent");
        var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var u in admins)
        {
            if (!string.IsNullOrWhiteSpace(u.Email))
            {
                emails.Add(u.Email);
            }
        }

        foreach (var u in agents)
        {
            if (!string.IsNullOrWhiteSpace(u.Email))
            {
                emails.Add(u.Email);
            }
        }

        if (exceptUserId is { } skipId)
        {
            var skip = await _userManager.FindByIdAsync(skipId.ToString());
            if (skip?.Email is not null)
            {
                emails.Remove(skip.Email);
            }
        }

        foreach (var email in emails)
        {
            await _hubContext.Clients
                .Group(NotificationHub.GroupNameForEmail(email))
                .SendAsync("notify", payload);
        }
    }
}
