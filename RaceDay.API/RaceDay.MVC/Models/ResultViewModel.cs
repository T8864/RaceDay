namespace RaceDay.MVC.Models
{
    public class ResultViewModel
    {
        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public string ParticipantName { get; set; } = "";
        public string EventName { get; set; } = "";
        public string FinishTime { get; set; } = "";
        public int Position { get; set; }
        public string? Notes { get; set; }
    }
}