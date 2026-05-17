using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VPAPortal.Data.Models
{
    public class PropertyInvoice
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string InvoiceNumber { get; set; } = "";

        public DateOnly Date { get; set; }

        [MaxLength(500)]
        public string? PhotoPath { get; set; }

        [MaxLength(200)]
        public string CreatedBy { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
