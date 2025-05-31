using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models;

public class UsedPart
{
        public int Id { get; set; }

        [Required]
        public int ServiceTaskId { get; set; }
        public ServiceTask ServiceTask { get; set; }

        [Required]
        public int PartId { get; set; }
        public Part Part { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Przechowujemy całkowity koszt = Part.UnitPrice * Quantity
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }
}