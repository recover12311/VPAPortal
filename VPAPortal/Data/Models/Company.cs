namespace VPAPortal.Data.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int SortOrder { get; set; }

        public ICollection<Crew> Crews { get; set; } = new List<Crew>();
        public ICollection<WarehouseDroneItem> WarehouseDrones { get; set; } = new List<WarehouseDroneItem>();
        public ICollection<WarehouseAmmoItem> WarehouseAmmos { get; set; } = new List<WarehouseAmmoItem>();
        public ICollection<WarehouseInvoice> Invoices { get; set; } = new List<WarehouseInvoice>();
        public ICollection<WarehouseLog> WarehouseLogs { get; set; } = new List<WarehouseLog>();
    }
}