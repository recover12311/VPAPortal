namespace VPAPortal.Data.Models
{
    public class WarehouseAmmoItem
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
    }
}