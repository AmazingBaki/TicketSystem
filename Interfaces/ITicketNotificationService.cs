namespace TicketSupportSystem.Interfaces;

public interface ITicketNotificationService
{
    Task NotifyNewTicketAsync(Guid ticketId, Guid creatorUserId, string title);
    Task NotifyNewCommentAsync(Guid ticketId, Guid commentAuthorUserId);
}
