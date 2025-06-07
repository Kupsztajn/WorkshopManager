using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models;

public class Comment
{
    [Required]
    public int Id { get; set; }
    
    public int ServiceOrderId { get; set; }
    public ServiceOrder ServiceOrder { get; set; }
    
    [Required]
    public string Author { get; set; }
    
    [Required, StringLength(2000)]
    public string Content { get; set; } = "";

    [Required] public DateTime Timestamp { get; set; }
}