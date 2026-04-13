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

            // Створюємо ролі
            string[] roles = ["SuperAdmin", "Admin", "Moderator", "Viewer"];
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

            // Створюємо SuperAdmin
            var adminUsername = "recover12311@gmail.com";

            // Шукаємо по UserName (не по Email)
            var adminUser = await userManager.FindByNameAsync(adminUsername);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminUsername,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, "Aa41824451@");
                if (!createResult.Succeeded)
                    throw new Exception("Помилка створення юзера: " +
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));

                var roleAssignResult = await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                if (!roleAssignResult.Succeeded)
                    throw new Exception("Помилка призначення ролі: " +
                        string.Join(", ", roleAssignResult.Errors.Select(e => e.Description)));
            }

            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (!db.Crews.Any())
            {
                db.Crews.AddRange(
                    new Crew { Name = "Екіпаж 1", SortOrder = 1 },
                    new Crew { Name = "Екіпаж 2", SortOrder = 2 },
                    new Crew { Name = "Екіпаж 3", SortOrder = 3 },
                    new Crew { Name = "Екіпаж 4", SortOrder = 4 }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}