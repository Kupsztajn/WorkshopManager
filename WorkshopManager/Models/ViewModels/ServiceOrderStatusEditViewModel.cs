using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models.ViewModels;

public class ServiceOrderStatusEditViewModel
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    [Display(Name = "Nowy status")]
    public string NewStatus { get; set; }

    // Aby móc wyświetlić obecny status w widoku (opcjonalnie)
    [Display(Name = "Aktualny status")]
    public string CurrentStatus { get; set; }

    // Lista dostępnych statusów do wyboru
    public IEnumerable<string> StatusesList { get; set; }
}