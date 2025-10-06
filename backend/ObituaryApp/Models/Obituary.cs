using System.ComponentModel.DataAnnotations;

namespace ObituaryApp.Models
{
    // Obituary model representing an obituary entry
    // This is the primary Entity Model for the application
    public class Obituary
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Date of Death")]
        [DataType(DataType.Date)]
        public DateTime DateOfDeath { get; set; }

        [Required]
        public string Biography { get; set; } = string.Empty;

        [Display(Name = "Photo")]
        public string? PhotoPath { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}