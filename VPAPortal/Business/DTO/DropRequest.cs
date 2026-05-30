using VPAPortal.Common;

namespace VPAPortal.Business.DTO
{
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
}
