namespace RaceDay.MVC.Models
{
    public class EventViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public double Distance { get; set; }
        public string? RouteDescription { get; set; }
        public string Status { get; set; } = "";
        public int MaxParticipants { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public int OrganiserID { get; set; }
    }
}