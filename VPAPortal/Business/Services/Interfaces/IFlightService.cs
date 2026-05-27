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

        /// <summary>Додає виліт Крила: нічого не списує.</summary>
        Task AddWingFlightAsync(WingFlightRequest request);

        /// <summary>Редагує базові поля вильоту (час, координати, ціль, результат, н.п.).</summary>
        Task UpdateFlightAsync(int flightId, TimeOnly time, string coordinates,
            string target, FlightResult result, string settlement);

        /// <summary>
        /// Видаляє виліт та повертає списані запаси назад.
        /// FPV: повертає дрон + боєкомплект.
        /// Бомбер: повертає тільки боєкомплект скидів.
        /// Крило: нічого не повертає.
        /// </summary>
        Task DeleteFlightAsync(int flightId, CrewType crewType);
    }

    // ── Request-об'єкти ─────────────────────────────────────────────────────

    /// <summary>Дані для створення вильоту FPV.</summary>
    public class FpvFlightRequest
    {
        public int CrewId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Coordinates { get; set; } = "";
        public string Target { get; set; } = "";
        public string Settlement { get; set; } = "";
        public int DroneItemId { get; set; }
        public int AmmoItemId { get; set; }
        public FlightResult Result { get; set; }
        public int TargetCount { get; set; }
        public string CreatedBy { get; set; } = "";
    }

    /// <summary>Дані для створення вильоту Бомбера.</summary>
    public class BomberFlightRequest
    {
        public int CrewId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int DroneItemId { get; set; }
        public bool DroneReturned { get; set; }
        public string MissionType { get; set; } = "attack"; // "attack" | "delivery"
        public List<DropRequest> Drops { get; set; } = new();
        public string CreatedBy { get; set; } = "";
    }

    /// <summary>Дані одного скиду / доставки.</summary>
    public class DropRequest
    {
        public string Coordinates { get; set; } = "";
        public string Target { get; set; } = "";
        public string Settlement { get; set; } = "";
        public int AmmoItemId { get; set; }
        public FlightResult Result { get; set; }
        public int TargetCount { get; set; }
        public TimeOnly DropTime { get; set; }
        public TimeOnly DeliveryTime { get; set; }
        public bool IsDelivery { get; set; }
    }

    /// <summary>Дані для створення вильоту Крила.</summary>
    public class WingFlightRequest
    {
        public int CrewId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Coordinates { get; set; } = "";
        public string Settlement { get; set; } = "";
        public int DroneItemId { get; set; }
        public bool DroneReturned { get; set; }
        public string CreatedBy { get; set; } = "";
    }
}