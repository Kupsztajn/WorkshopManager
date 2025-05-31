using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models.ViewModels;

public class UsedPartCreateViewModel
{
    [Required]
    public int ServiceTaskId { get; set; }

    [Required]
    public int PartId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Ilość musi być >= 1")]
    public int Quantity { get; set; }
}