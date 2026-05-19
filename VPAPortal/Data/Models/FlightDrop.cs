using VPAPortal.Common;

namespace VPAPortal.Data.Models
{
    public class FlightDrop
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;
        public string Coordinates { get; set; } = "";
        public string Target { get; set; } = "";
        public int? AmmoItemId { get; set; }
        public AmmoItem? AmmoItem { get; set; }
        public FlightResult Result { get; set; }
        public int TargetCount { get; set; }
        public bool IsDelivery { get; set; }
        public TimeOnly? DeliveryTime { get; set; }
        public string Settlement { get; set; } = "";
    }
}