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

            // Ensure database is created
            context.Database.EnsureCreated();

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
        }
    }
}