using System.ComponentModel.DataAnnotations;

namespace frontend.Models
{
    public class Obituary
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfDeath { get; set; }

        [Required(ErrorMessage = "Biography is required")]
        public string Biography { get; set; } = string.Empty;

        public string? PhotoPath { get; set; }
        public string? SubmittedByName { get; set; }
        public string? CreatedByEmail { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
