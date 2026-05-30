using VPAPortal.Common;

namespace VPAPortal.Business.DTO
{
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
}
