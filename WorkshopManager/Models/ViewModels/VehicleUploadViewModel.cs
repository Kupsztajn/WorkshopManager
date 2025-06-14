﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WorkshopManager.Models.ViewModels;

public class VehicleUploadViewModel
{
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
    public IFormFile Photo { get; set; }  // tu wczytasz plik z formularza
}