using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models.ViewModels;

public class ServiceTaskCreateViewModel
{
        public int ServiceOrderId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required] [Range(0, 100000)] 
        public decimal LaborCost { get; set; }
}