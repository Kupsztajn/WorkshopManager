using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models;

public class ServiceOrder
{
    public int Id { get; set; }

    [Required]
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Status { get; set; } = "Nowe";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    // przypisany mechanik (ApplicationUser.Id)
    public string MechanicId { get; set; }
    public ApplicationUser Mechanic { get; set; }
    
    public ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();
}