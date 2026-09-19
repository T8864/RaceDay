using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        [MaxLength(20)]
        public string FinishTime { get; set; } = string.Empty;

        public int? PositionOverall { get; set; }

        public int? PositionCategory { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Foreign Key
        public int EnrolmentID { get; set; }

        [ForeignKey("EnrolmentID")]
        public Enrolment Enrolment { get; set; } = null!;
    }
}