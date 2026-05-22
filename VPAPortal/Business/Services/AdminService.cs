using Microsoft.AspNetCore.Identity;
using VPAPortal.Business.Services.Interfaces;
using VPAPortal.Data;

namespace VPAPortal.Business.Services
{
    /// <summary>
    /// Реалізація сервісу управління користувачами та ролями.
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <inheritdoc />
        public Task<List<string>> GetAvailableRolesAsync()
        {
            var roles = _roleManager.Roles
                .Where(r => r.Name != "Admin")
                .Select(r => r.Name!)
                .ToList();

            return Task.FromResult(roles);
        }

        /// <inheritdoc />
        public Task<List<ApplicationUser>> GetUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return Task.FromResult(users);
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, List<string>>> GetUserRolesAsync(
            List<ApplicationUser> users)
        {
            var result = new Dictionary<string, List<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result[user.Id] = roles.ToList();
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IdentityResult> CreateUserAsync(
            string username,
            string password,
            string role)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = null,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        /// <inheritdoc />
        public async Task<IdentityResult> ChangePasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Користувача не знайдено." }
                );
            }

            // Генеруємо токен скидання пароля і відразу застосовуємо
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}