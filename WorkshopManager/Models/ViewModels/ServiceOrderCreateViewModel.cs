using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models.ViewModels;

public class ServiceOrderCreateViewModel
{
    [Required]
    public int VehicleId { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string MechanicId { get; set; }

    public string Status { get; set; } = "Nowe";
}