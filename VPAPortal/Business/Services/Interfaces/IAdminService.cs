using Microsoft.AspNetCore.Identity;
using VPAPortal.Data;

namespace VPAPortal.Business.Services.Interfaces
{
    /// <summary>
    /// Сервіс для управління користувачами та ролями.
    /// </summary>
    public interface IAdminService
    {
        /// <summary>Повертає список всіх ролей, крім "Admin".</summary>
        Task<List<string>> GetAvailableRolesAsync();

        /// <summary>Повертає список всіх користувачів.</summary>
        Task<List<ApplicationUser>> GetUsersAsync();

        /// <summary>Повертає словник userId → список ролей для заданих користувачів.</summary>
        Task<Dictionary<string, List<string>>> GetUserRolesAsync(List<ApplicationUser> users);

        /// <summary>Створює нового користувача з заданою роллю.</summary>
        Task<IdentityResult> CreateUserAsync(string username, string password, string role);

        /// <summary>Видаляє користувача за його Id.</summary>
        Task DeleteUserAsync(string userId);

        /// <summary>Змінює пароль користувача за його Id.</summary>
        Task<IdentityResult> ChangePasswordAsync(string userId, string newPassword);
    }
}