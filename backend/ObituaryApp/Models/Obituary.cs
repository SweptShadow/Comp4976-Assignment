using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Navigation property for the user who created this obituary
        public virtual ApplicationUser? CreatedByUser { get; set; }

        // For non-authenticated users submitting obituaries
        [Display(Name = "Submitted By")]
        public string? SubmittedByName { get; set; }

        // Computed property for display name
        [NotMapped]
        public string CreatedByDisplayName =>
            !string.IsNullOrEmpty(SubmittedByName) ? SubmittedByName :
            (!string.IsNullOrEmpty(CreatedByUser?.Email) ? CreatedByUser!.Email! :
            (!string.IsNullOrEmpty(CreatedByUser?.UserName) ? CreatedByUser!.UserName! :
            "Unknown User"));

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
