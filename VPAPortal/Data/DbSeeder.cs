using Microsoft.AspNetCore.Identity;
using VPAPortal.Data.Models;

namespace VPAPortal.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roles = ["SuperAdmin", "Moderator", "Viewer"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                        throw new Exception($"Помилка створення ролі '{role}': " +
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            var adminUsername = "admin1234567";
            var adminUser = await userManager.FindByNameAsync(adminUsername);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminUsername,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(adminUser, "Qwerty1234567@");
                if (!createResult.Succeeded)
                    throw new Exception("Помилка створення юзера: " +
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));

                await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
            }

            // Seed рот
            if (!db.Companies.Any())
            {
                db.Companies.AddRange(
                    new Company { Name = "1 рота", SortOrder = 1 },
                    new Company { Name = "2 рота", SortOrder = 2 },
                    new Company { Name = "3 рота", SortOrder = 3 }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}