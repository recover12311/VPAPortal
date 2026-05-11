namespace VPAPortal.Data.Models
{
    public class PropertyItem
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public Crew Crew { get; set; } = null!;
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public int PropertyTypeId { get; set; }
        public PropertyType PropertyType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
    }
}
