namespace RaceDay.API.DTOs
{
    public class CreateResultDto
    {
        public int EnrolmentID { get; set; }
        public string? FinishTime { get; set; }
        public int? PositionOverall { get; set; }
        public int? PositionCategory { get; set; }
        public string? Notes { get; set; }
    }
}