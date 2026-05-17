using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VPAPortal.Data.Models
{
    public class PropertyLog
    {
        public int Id { get; set; }

        // ── Прив'язка ─────────────────────────────────────────────────────
        [Required]
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        public int? CrewId { get; set; }

        [ForeignKey(nameof(CrewId))]
        public Crew? Crew { get; set; }

        // ── Зміст запису ─────────────────────────────────────────────────
        /// <summary>"Рота" або "Екіпаж"</summary>
        [MaxLength(20)]
        public string Level { get; set; } = "Екіпаж";

        [MaxLength(50)]
        public string Action { get; set; } = "";

        [MaxLength(200)]
        public string ItemName { get; set; } = "";

        [MaxLength(500)]
        public string Details { get; set; } = "";

        [MaxLength(200)]
        public string CreatedBy { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
