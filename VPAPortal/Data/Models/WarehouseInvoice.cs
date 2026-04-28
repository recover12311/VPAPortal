namespace VPAPortal.Data.Models
{
    public class WarehouseInvoice
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public string InvoiceNumber { get; set; } = "";
        public DateOnly Date { get; set; }
        public string? PhotoPath { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}