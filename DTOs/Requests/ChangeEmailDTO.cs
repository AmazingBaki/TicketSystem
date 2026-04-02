namespace TicketSupportSystem.DTOs.Requests
{
    public class ChangeEmailDTO
    {
        public string NewEmail { get; set; } = null!;
        public string CurrentPassword { get; set; } = null!;
    }
}

