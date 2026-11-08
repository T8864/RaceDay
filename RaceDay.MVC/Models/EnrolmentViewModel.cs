namespace RaceDay.MVC.Models
{
    public class EnrolmentViewModel
    {
        public int EnrolmentID { get; set; }
        public int EventID { get; set; }
        public string EventName { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public double Distance { get; set; }
        public int UserID { get; set; }
        public string ParticipantName { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime EnrolmentDate { get; set; }
    }
}