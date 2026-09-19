namespace RaceDay.API.DTOs
{
    public class CreateEventDto
    {
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Distance { get; set; } = string.Empty;
        public string? RouteDescription { get; set; }
        public int MaxParticipants { get; set; }
        public int CategoryID { get; set; }
    }

    public class UpdateEventDto
    {
        public string? EventName { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Location { get; set; }
        public string? Distance { get; set; }
        public string? Status { get; set; }
        public int? MaxParticipants { get; set; }
    }
}