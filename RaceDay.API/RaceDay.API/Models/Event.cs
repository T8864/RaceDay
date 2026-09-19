using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [MaxLength(150)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Distance { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? RouteDescription { get; set; }

        [Required]
        public string Status { get; set; } = "Upcoming";

        public int MaxParticipants { get; set; } = 100;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key
        public int OrganiserID { get; set; }

        [ForeignKey("OrganiserID")]
        public User Organiser { get; set; } = null!;

        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; } = null!;

        // Navigation properties
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}