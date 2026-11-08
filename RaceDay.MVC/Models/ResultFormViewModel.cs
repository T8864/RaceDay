using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class ResultFormViewModel
    {
        public int ResultID { get; set; }

        [Required]
        [Display(Name = "Enrolment ID")]
        public int EnrolmentID { get; set; }

        [Required]
        [Display(Name = "Finish Time")]
        public string FinishTime { get; set; } = "";

        [Required]
        [Range(1, 10000)]
        public int Position { get; set; }

        public string? Notes { get; set; }
    }
}