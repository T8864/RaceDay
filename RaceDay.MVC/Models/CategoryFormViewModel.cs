using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CategoryFormViewModel
    {
        public int CategoryID { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = "";

        public string? Description { get; set; }
    }
}