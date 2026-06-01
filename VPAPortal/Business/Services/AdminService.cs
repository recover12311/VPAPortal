using Microsoft.AspNetCore.Identity;
using VPAPortal.Business.Services.Interfaces;
using VPAPortal.Data;

namespace VPAPortal.Business.Services
{
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

        public Task<List<string>> GetAvailableRolesAsync() =>
            Task.FromResult(
                _roleManager.Roles
                    .Where(r => r.Name != "Admin")
                    .Select(r => r.Name!)
                    .ToList());

        public Task<List<ApplicationUser>> GetUsersAsync() =>
            Task.FromResult(_userManager.Users.ToList());

        public async Task<Dictionary<string, List<string>>> GetUserRolesAsync(
            List<ApplicationUser> users)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var user in users)
                result[user.Id] = (await _userManager.GetRolesAsync(user)).ToList();
            return result;
        }

        public async Task<IdentityResult> CreateUserAsync(string username, string password, string role)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = null,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, role);
            return result;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(
                    new IdentityError { Description = "Користувача не знайдено." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}
