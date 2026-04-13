namespace VPAPortal.Data.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public Crew Crew { get; set; } = null!;
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Coordinates { get; set; } = "";
        public int DroneItemId { get; set; }
        public DroneItem DroneItem { get; set; } = null!;
        public int AmmoItemId { get; set; }
        public AmmoItem AmmoItem { get; set; } = null!;
        public FlightResult Result { get; set; }
        public string Settlement { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
