using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "Confirmed";

        // Foreign Keys
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; } = null!;

        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public Event Event { get; set; } = null!;

        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; } = null!;

        // Navigation property
        public Result? Result { get; set; }
    }
}