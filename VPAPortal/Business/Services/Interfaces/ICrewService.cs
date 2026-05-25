using VPAPortal.Common;
using VPAPortal.Data.Models;

namespace VPAPortal.Business.Services.Interfaces
{
    /// <summary>
    /// Сервіс для управління екіпажами: дрони, бомбери, крила,
    /// боєкомплект, передача зі складу, журнал.
    /// </summary>
    public interface ICrewService
    {
        // ── Екіпажі ─────────────────────────────────────────────────────────

        /// <summary>Повертає список екіпажів для роти.</summary>
        Task<List<Crew>> GetCrewsAsync(int companyId);

        /// <summary>Додає новий екіпаж.</summary>
        Task<Crew> AddCrewAsync(int companyId, string name, CrewType type, int maxCurrentOrder);

        /// <summary>Видаляє екіпаж з усіма пов'язаними даними.</summary>
        Task DeleteCrewAsync(int crewId);

        // ── Безпілотники екіпажу ─────────────────────────────────────────────

        /// <summary>Повертає всі DroneItem для екіпажу (FPV + Бомбери + Крила).</summary>
        Task<List<DroneItem>> GetDroneItemsAsync(int crewId);

        /// <summary>
        /// Додає безпілотник або збільшує кількість існуючого.
        /// isBomber/isWing визначає тип апарату.
        /// </summary>
        Task AddDroneItemAsync(int crewId, string name, int qty,
            bool isBomber, bool isWing, string changedBy);

        /// <summary>Зберігає зміни запису безпілотника.</summary>
        Task SaveDroneItemAsync(int itemId, string name, int qty, string changedBy);

        /// <summary>Видаляє запис безпілотника.</summary>
        Task DeleteDroneItemAsync(int itemId, string changedBy);

        // ── Боєкомплект екіпажу ──────────────────────────────────────────────

        /// <summary>Повертає боєкомплект для екіпажу.</summary>
        Task<List<AmmoItem>> GetAmmoItemsAsync(int crewId);

        /// <summary>Додає боєкомплект або збільшує кількість існуючого.</summary>
        Task AddAmmoItemAsync(int crewId, string name, int qty, string changedBy);

        /// <summary>Зберігає зміни боєкомплекту.</summary>
        Task SaveAmmoItemAsync(int itemId, string name, int qty, string changedBy);

        /// <summary>Видаляє боєкомплект.</summary>
        Task DeleteAmmoItemAsync(int itemId, string changedBy);

        // ── Передача зі складу ───────────────────────────────────────────────

        /// <summary>
        /// Повертає безпілотники складу для роти з кількістю > 0.
        /// </summary>
        Task<List<WarehouseDroneItem>> GetWarehouseDronesAsync(int companyId);

        /// <summary>
        /// Повертає боєкомплект складу для роти з кількістю > 0.
        /// </summary>
        Task<List<WarehouseAmmoItem>> GetWarehouseAmmosAsync(int companyId);

        /// <summary>
        /// Передає безпілотник зі складу до екіпажу.
        /// transferType: "drone" | "bomber" | "wing"
        /// </summary>
        Task<string> TransferDroneFromWarehouseAsync(
            int companyId, int crewId, int warehouseItemId,
            int qty, string transferType, string changedBy);

        /// <summary>Передає боєкомплект зі складу до екіпажу.</summary>
        Task<string> TransferAmmoFromWarehouseAsync(
            int companyId, int crewId, int warehouseItemId,
            int qty, string changedBy);

        // ── Журнал ───────────────────────────────────────────────────────────

        /// <summary>Повертає журнал змін для екіпажу.</summary>
        Task<List<CrewLog>> GetLogsAsync(int crewId);
    }
}