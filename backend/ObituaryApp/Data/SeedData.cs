using Microsoft.AspNetCore.Identity;
using ObituaryApp.Models;

namespace ObituaryApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles
            if (!await roleManager.RoleExistsAsync("admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (!await roleManager.RoleExistsAsync("user"))
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            // Create admin user
            if (await userManager.FindByEmailAsync("aa@aa.aa") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "aa@aa.aa",
                    Email = "aa@aa.aa",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }
            }

            // Create regular user
            if (await userManager.FindByEmailAsync("uu@uu.uu") == null)
            {
                var regularUser = new ApplicationUser
                {
                    UserName = "uu@uu.uu",
                    Email = "uu@uu.uu",
                    FirstName = "Regular",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(regularUser, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "user");
                }
            }

            // Seed obituaries
            if (!context.Obituaries.Any())
            {
                var adminUser = await userManager.FindByEmailAsync("aa@aa.aa");
                if (adminUser != null)
                {
                    var obituaries = new[]
                    {
                        new Obituary
                        {
                            FullName = "Albert Einstein",
                            DateOfBirth = new DateTime(1879, 3, 14),
                            DateOfDeath = new DateTime(1955, 4, 18),
                            Biography = "Theoretical physicist who developed the theory of relativity, one of the two pillars of modern physics. Best known for his mass-energy equivalence formula E = mc². Awarded the Nobel Prize in Physics in 1921.",
                            CreatedBy = adminUser.Id,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        },
                        new Obituary
                        {
                            FullName = "Isaac Newton",
                            DateOfBirth = new DateTime(1643, 1, 4),
                            DateOfDeath = new DateTime(1727, 3, 31),
                            Biography = "Mathematician, physicist, astronomer, and author widely recognized as one of the greatest mathematicians and physicists of all time. Formulated the laws of motion and universal gravitation.",
                            CreatedBy = adminUser.Id,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        },
                        new Obituary
                        {
                            FullName = "Marie Curie",
                            DateOfBirth = new DateTime(1867, 11, 7),
                            DateOfDeath = new DateTime(1934, 7, 4),
                            Biography = "Polish and naturalized-French physicist and chemist who conducted pioneering research on radioactivity. First woman to win a Nobel Prize, first person to win a Nobel Prize twice, and the only person to win in two scientific fields.",
                            CreatedBy = adminUser.Id,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        },
                        new Obituary
                        {
                            FullName = "Leonardo da Vinci",
                            DateOfBirth = new DateTime(1452, 4, 15),
                            DateOfDeath = new DateTime(1519, 5, 2),
                            Biography = "Italian polymath of the High Renaissance who was active as a painter, draughtsman, engineer, scientist, theorist, sculptor, and architect. Known for masterpieces including the Mona Lisa and The Last Supper.",
                            CreatedBy = adminUser.Id,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        },
                        new Obituary
                        {
                            FullName = "Ada Lovelace",
                            DateOfBirth = new DateTime(1815, 12, 10),
                            DateOfDeath = new DateTime(1852, 11, 27),
                            Biography = "English mathematician and writer, chiefly known for her work on Charles Babbage's proposed mechanical general-purpose computer, the Analytical Engine. Recognized as the first computer programmer.",
                            CreatedBy = adminUser.Id,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        }
                    };

                    context.Obituaries.AddRange(obituaries);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}