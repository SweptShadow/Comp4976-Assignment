using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ObituaryApp.Models;

namespace ObituaryApp.Data
{
    // ApplicationDbContext = Your Database Manager
    // - Handles all database operations (CRUD)
    // - Converts C# objects to/from SQLite database
    // - IdentityDbContext gives you user/role tables automatically
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet properties = Database Tables (Code First approach)
        public DbSet<Obituary> Obituaries { get; set; }  // Creates "Obituaries" table
        // Future: public DbSet<Photo> Photos { get; set; }  // Would create "Photos" table

        // OnModelCreating = Configure database schema and relationships
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);  // Configure Identity tables first

            // Code First: Configure entity properties using Fluent API
            builder.Entity<Obituary>()
                .Property(o => o.FullName)
                .HasMaxLength(200);  // SQL: FullName VARCHAR(200)

            builder.Entity<Obituary>()
                .Property(o => o.Biography)
                .HasMaxLength(5000);  // SQL: Biography VARCHAR(5000)

            // Add index for faster search by name
            builder.Entity<Obituary>()
                .HasIndex(o => o.FullName);

            // Future relationship example:
            // builder.Entity<Obituary>()
            //     .HasMany(o => o.Photos)
            //     .WithOne(p => p.Obituary)
            //     .HasForeignKey(p => p.ObituaryId);
        }
    }
}