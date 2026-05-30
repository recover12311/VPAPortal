using VPAPortal.Business.DTO;
using VPAPortal.Common;
using VPAPortal.Data.Models;

namespace VPAPortal.Business.Services.Interfaces
{
    /// <summary>
    /// Сервіс для управління вильотами всіх типів екіпажів.
    /// </summary>
    public interface IFlightService
    {
        // ── Довідники ────────────────────────────────────────────────────────

        /// <summary>Повертає FPV дрони екіпажу з кількістю > 0.</summary>
        Task<List<DroneItem>> GetAvailableDronesAsync(int crewId);

        /// <summary>Повертає бомбери екіпажу (незалежно від кількості).</summary>
        Task<List<DroneItem>> GetAvailableBombersAsync(int crewId);

        /// <summary>Повертає крила екіпажу (незалежно від кількості).</summary>
        Task<List<DroneItem>> GetAvailableWingsAsync(int crewId);

        /// <summary>Повертає ударні крила екіпажу (незалежно від кількості).</summary>
        Task<List<DroneItem>> GetAvailableWingsAttackAsync(int crewId);

        /// <summary>Повертає боєкомплект екіпажу з кількістю > 0.</summary>
        Task<List<AmmoItem>> GetAvailableAmmosAsync(int crewId);

        // ── Вильоти ──────────────────────────────────────────────────────────

        /// <summary>
        /// Повертає вильоти екіпажу за конкретну дату
        /// з підвантаженням DroneItem та Drops → AmmoItem.
        /// </summary>
        Task<List<Flight>> GetFlightsAsync(int crewId, DateOnly date);

        /// <summary>Додає виліт FPV: списує дрон і боєкомплект.</summary>
        Task AddFpvFlightAsync(FpvFlightRequest request);

        /// <summary>Додає виліт Бомбера: списує боєкомплект скидів.</summary>
        Task AddBomberFlightAsync(BomberFlightRequest request);

        /// <summary>Додає виліт Крила (розвідка): нічого не списує.</summary>
        Task AddWingFlightAsync(WingFlightRequest request);

        /// <summary>Додає виліт Ударного крила: списує боєкомплект скидів.</summary>
        Task AddWingAttackFlightAsync(BomberFlightRequest request);

        /// <summary>Редагує базові поля вильоту (час, координати, ціль, результат, н.п.).</summary>
        Task UpdateFlightAsync(int flightId, TimeOnly time, string coordinates,
            string target, FlightResult result, string settlement);

        /// <summary>
        /// Видаляє виліт та повертає списані запаси назад.
        /// FPV: повертає дрон + боєкомплект.
        /// Бомбер / КрилоУдарне: повертає тільки боєкомплект скидів.
        /// Крило: нічого не повертає.
        /// </summary>
        Task DeleteFlightAsync(int flightId, CrewType crewType);
    }
}