using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class EventFormViewModel
    {
        public int EventID { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        public string EventName { get; set; } = "";

        [Required]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; } = DateTime.Today.AddDays(30);

        [Required]
        public string Location { get; set; } = "";

        [Required]
        [Range(0.1, 1000)]
        public double Distance { get; set; }

        [Display(Name = "Route Description")]
        public string? RouteDescription { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Max Participants")]
        public int MaxParticipants { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        public string Status { get; set; } = "Open";
    }
}