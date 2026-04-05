using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TicketSupportSystem.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public const string HubPath = "/hubs/notifications";

    public static string GroupNameForEmail(string email)
    {
        return $"user:{email.Trim().ToLowerInvariant()}";
    }

    public override async Task OnConnectedAsync()
    {
        var email = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(email))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupNameForEmail(email));
        }

        await base.OnConnectedAsync();
    }
}
