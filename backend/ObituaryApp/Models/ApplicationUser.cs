using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ObituaryApp.Models
{
    // User model that extends IdentityUser
    // IdentityUser (ASP.NET Core Identity) gives us: Id, UserName, Email, PasswordHash, Roles, etc.
    // ApplicationUser we add: FirstName, LastName
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        // Computed property for full name display
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}