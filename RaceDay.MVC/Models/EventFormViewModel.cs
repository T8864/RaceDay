using Microsoft.AspNetCore.Http;
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
        public DateTime EventDate { get; set; } = DateTime.Today;

        [Required]
        public string Location { get; set; } = "";

        [Required]
        public double Distance { get; set; }

        [Display(Name = "Route Description")]
        public string? RouteDescription { get; set; }

        [Required]
        [Display(Name = "Max Participants")]
        public int MaxParticipants { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        public string Status { get; set; } = "Upcoming";

        [Display(Name = "Event Image")]
        public IFormFile? ImageFile { get; set; }
    }
}