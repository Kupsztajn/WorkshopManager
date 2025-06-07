using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkshopManager.Models.ViewModels;

public class PartCreateViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }
}