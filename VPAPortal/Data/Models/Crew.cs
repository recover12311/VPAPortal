namespace VPAPortal.Data.Models
{
    public class Crew
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public string Name { get; set; } = "";
        public CrewType Type { get; set; }
        public int SortOrder { get; set; }

        public ICollection<DroneItem> DroneItems { get; set; } = new List<DroneItem>();
        public ICollection<AmmoItem> AmmoItems { get; set; } = new List<AmmoItem>();
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public ICollection<CrewLog> CrewLogs { get; set; } = new List<CrewLog>();
    }
}