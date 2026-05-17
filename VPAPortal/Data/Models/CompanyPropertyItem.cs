using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VPAPortal.Data.Models
{
    public class CompanyPropertyItem
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        public int PropertyTypeId { get; set; }

        [ForeignKey(nameof(PropertyTypeId))]
        public PropertyType PropertyType { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = "";

        public int Quantity { get; set; }

        [MaxLength(200)]
        public string CreatedBy { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
