using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models;

public class ServiceTask
{
        public int Id { get; set; }

        [Required]
        public int ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LaborCost { get; set; }
        
        // Dodaj nawigację do listy części
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
}