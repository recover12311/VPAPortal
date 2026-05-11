namespace VPAPortal.Data.Models
{
    public class PropertyLog
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public string Action { get; set; } = ""; // "Додано", "Змінено", "Видалено"
        public string ItemName { get; set; } = "";
        public string Details { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
    }
}
