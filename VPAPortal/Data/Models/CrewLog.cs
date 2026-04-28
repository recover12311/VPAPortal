namespace VPAPortal.Data.Models
{
    public class CrewLog
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public Crew Crew { get; set; } = null!;
        public string ItemType { get; set; } = "";
        public string ItemName { get; set; } = "";
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public string Action { get; set; } = "";
        public string ChangedBy { get; set; } = "";
        public DateTime ChangedAt { get; set; }
    }
}