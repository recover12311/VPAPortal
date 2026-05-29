using VPAPortal.Data.Models;

namespace VPAPortal.Business.Services.Interfaces
{
    /// <summary>
    /// Сервіс для управління майном рот та екіпажів,
    /// накладними та журналом змін.
    /// </summary>
    public interface IPropertyService
    {
        // ── Типи майна ───────────────────────────────────────────────────────

        /// <summary>Повертає всі типи майна, відсортовані за SortOrder.</summary>
        Task<List<PropertyType>> GetPropertyTypesAsync();

        /// <summary>Додає новий тип майна.</summary>
        Task AddPropertyTypeAsync(string name, int maxCurrentOrder);

        /// <summary>Зберігає зміни типу майна.</summary>
        Task SavePropertyTypeAsync(int typeId, string name);

        /// <summary>
        /// Видаляє тип майна.
        /// Повертає false якщо тип використовується — видалення заблоковано.
        /// </summary>
        Task<bool> DeletePropertyTypeAsync(int typeId);

        // ── Майно РОТИ ───────────────────────────────────────────────────────

        /// <summary>Повертає майно роти з підвантаженням PropertyType.</summary>
        Task<List<CompanyPropertyItem>> GetCompanyItemsAsync(int companyId);

        /// <summary>Додає нову позицію майна роти. Записує в журнал.</summary>
        Task AddCompanyItemAsync(int companyId, string name, int qty,
            int typeId, string createdBy);

        /// <summary>Зберігає зміни позиції майна роти. Записує в журнал.</summary>
        Task SaveCompanyItemAsync(int itemId, string name, int qty,
            int typeId, string changedBy);

        /// <summary>Видаляє позицію майна роти. Записує в журнал.</summary>
        Task DeleteCompanyItemAsync(int itemId, string changedBy);

        // ── Передача роти → екіпаж ───────────────────────────────────────────

        /// <summary>
        /// Передає майно з роти екіпажу.
        /// Зменшує кількість у роти, збільшує (або створює) у екіпажу.
        /// Записує в журнал.
        /// Повертає текст повідомлення про результат.
        /// </summary>
        Task<string> TransferToCrewAsync(int companyId, int crewId,
            int companyItemId, int qty, string changedBy);

        // ── Майно ЕКІПАЖУ ────────────────────────────────────────────────────

        /// <summary>Повертає майно екіпажу з підвантаженням PropertyType.</summary>
        Task<List<PropertyItem>> GetCrewItemsAsync(int crewId);

        /// <summary>Додає нову позицію майна екіпажу. Записує в журнал.</summary>
        Task AddCrewItemAsync(int crewId, int companyId, string name,
            int qty, int typeId, string crewName, string createdBy);

        /// <summary>Зберігає зміни позиції майна екіпажу. Записує в журнал.</summary>
        Task SaveCrewItemAsync(int itemId, string name, int qty,
            int typeId, string changedBy);

        /// <summary>Видаляє позицію майна екіпажу. Записує в журнал.</summary>
        Task DeleteCrewItemAsync(int itemId, string crewName, string changedBy);

        // ── Накладні ────────────────────────────────────────────────────────

        /// <summary>Повертає накладні для роти.</summary>
        Task<List<PropertyInvoice>> GetInvoicesAsync(int companyId);

        /// <summary>Додає нову накладну.</summary>
        Task AddInvoiceAsync(int companyId, string invoiceNumber,
            DateOnly date, string? photoPath, string createdBy);

        /// <summary>Видаляє накладну та файл якщо він є.</summary>
        Task DeleteInvoiceAsync(int invoiceId, string webRootPath);

        // ── Журнал ───────────────────────────────────────────────────────────

        /// <summary>Повертає журнал змін для роти.</summary>
        Task<List<PropertyLog>> GetLogsAsync(int companyId);
    }
}