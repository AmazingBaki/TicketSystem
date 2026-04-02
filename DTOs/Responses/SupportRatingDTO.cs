namespace TicketSupportSystem.DTOs.Responses
{
    public class SupportRatingDTO
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public string AgentEmail { get; set; } = string.Empty;

        public int AssignedTotal { get; set; }
        public int ClosedTotal { get; set; }

        public double? AvgFirstResponseMinutes { get; set; }
        public double? AvgResolutionHours { get; set; }
    }
}

