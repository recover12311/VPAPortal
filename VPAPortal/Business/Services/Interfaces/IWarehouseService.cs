using VPAPortal.Data.Models;

namespace VPAPortal.Business.Services.Interfaces
{
    /// <summary>
    /// Сервіс для управління складом: безпілотники, боєкомплект, накладні, журнал.
    /// </summary>
    public interface IWarehouseService
    {
        // ── Роти ────────────────────────────────────────────────────────────

        /// <summary>Повертає список всіх рот, відсортованих за SortOrder.</summary>
        Task<List<Company>> GetCompaniesAsync();

        /// <summary>Додає нову роту.</summary>
        Task<Company> AddCompanyAsync(string name, int maxCurrentOrder);

        /// <summary>Видаляє роту та всі пов'язані дані складу.</summary>
        Task DeleteCompanyAsync(int companyId);

        // ── Безпілотники складу ─────────────────────────────────────────────

        /// <summary>Повертає список безпілотників для заданої роти.</summary>
        Task<List<WarehouseDroneItem>> GetDronesAsync(int companyId);

        /// <summary>
        /// Додає безпілотник або збільшує кількість існуючого.
        /// Записує в журнал.
        /// </summary>
        Task AddDroneAsync(int companyId, string name, int qty, string changedBy);

        /// <summary>Зберігає зміни безпілотника. Записує в журнал.</summary>
        Task SaveDroneAsync(int itemId, string name, int qty, string changedBy);

        /// <summary>Видаляє безпілотник. Записує в журнал.</summary>
        Task DeleteDroneAsync(int itemId, string changedBy);

        // ── Боєкомплект складу ──────────────────────────────────────────────

        /// <summary>Повертає список боєкомплекту для заданої роти.</summary>
        Task<List<WarehouseAmmoItem>> GetAmmosAsync(int companyId);

        /// <summary>
        /// Додає боєкомплект або збільшує кількість існуючого.
        /// Записує в журнал.
        /// </summary>
        Task AddAmmoAsync(int companyId, string name, int qty, string changedBy);

        /// <summary>Зберігає зміни боєкомплекту. Записує в журнал.</summary>
        Task SaveAmmoAsync(int itemId, string name, int qty, string changedBy);

        /// <summary>Видаляє боєкомплект. Записує в журнал.</summary>
        Task DeleteAmmoAsync(int itemId, string changedBy);

        // ── Накладні ────────────────────────────────────────────────────────

        /// <summary>Повертає список накладних для заданої роти.</summary>
        Task<List<WarehouseInvoice>> GetInvoicesAsync(int companyId);

        /// <summary>Додає нову накладну.</summary>
        Task AddInvoiceAsync(int companyId, string invoiceNumber, DateOnly date,
            string? photoPath, string createdBy);

        /// <summary>Видаляє накладну та файл, якщо він є.</summary>
        Task DeleteInvoiceAsync(int invoiceId, string webRootPath);

        // ── Журнал ──────────────────────────────────────────────────────────

        /// <summary>Повертає журнал змін для заданої роти.</summary>
        Task<List<WarehouseLog>> GetLogsAsync(int companyId);
    }
}
