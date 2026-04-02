namespace TicketSupportSystem.DTOs.Responses
{
    public class CurrentUserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
