namespace VPAPortal.Business.DTO
{
    /// <summary>Дані для створення вильоту Крила (розвідка).</summary>
    public class WingFlightRequest
    {
        public int CrewId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Coordinates { get; set; } = "";
        public string Settlement { get; set; } = "";
        public int DroneItemId { get; set; }
        public bool DroneReturned { get; set; }

        /// <summary>Причина незворотності з фіксованого списку. Null якщо повернувся.</summary>
        public string? DroneNotReturnedReason { get; set; }

        /// <summary>Довільна причина, якщо обрано "Інше".</summary>
        public string? DroneNotReturnedCustom { get; set; }

        public string CreatedBy { get; set; } = "";
    }
}
