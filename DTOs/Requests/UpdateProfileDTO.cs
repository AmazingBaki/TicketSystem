namespace TicketSupportSystem.DTOs.Requests
{
    public class UpdateProfileDTO
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}

