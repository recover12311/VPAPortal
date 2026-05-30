using VPAPortal.Business.Services.Interfaces;

namespace VPAPortal.Business.DTO
{
    /// <summary>Дані для створення вильоту Бомбера або Ударного крила.</summary>
    public class BomberFlightRequest
    {
        public int CrewId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int DroneItemId { get; set; }
        public bool DroneReturned { get; set; }

        /// <summary>Причина незворотності з фіксованого списку. Null якщо повернувся.</summary>
        public string? DroneNotReturnedReason { get; set; }

        /// <summary>Довільна причина, якщо обрано "Інше".</summary>
        public string? DroneNotReturnedCustom { get; set; }

        public string MissionType { get; set; } = "attack"; // "attack" | "delivery"
        public List<DropRequest> Drops { get; set; } = new();
        public string CreatedBy { get; set; } = "";
    }
}
