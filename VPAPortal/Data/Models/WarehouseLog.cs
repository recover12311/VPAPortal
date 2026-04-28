namespace VPAPortal.Data.Models
{
    public class WarehouseLog
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public string ItemType { get; set; } = "";   // "Дрон" / "Боєкомплект"
        public string ItemName { get; set; } = "";
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public string Action { get; set; } = "";     // "Додано" / "Змінено" / "Видалено"
        public string ChangedBy { get; set; } = "";
        public DateTime ChangedAt { get; set; }
    }
}