namespace VPAPortal.Data.Models
{
    public class DroneItem
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public Crew Crew { get; set; } = null!;
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
    }
}
