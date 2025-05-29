using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WorkshopManager.Models;

public class Vehicle
{
    public int Id { get; set; }
    
    public string Brand { get; set; }        // marka
    public string Model { get; set; }        // model
    public string VIN { get; set; }          // numer VIN
    public string Registration { get; set; } // numer rejestracyjny
    public int Year { get; set; }            // rok produkcji

    // Klucz obcy do klienta
    public string ClientId { get; set; }
    [BindNever]
    public ApplicationUser Client { get; set; }  // nawiązanie do klienta (jeśli klient to ApplicationUser)
    
    public string ImageUrl { get; set; }
    
    public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
}
