using Microsoft.AspNetCore.Identity;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services.Implementations;

public class MechanicService : IMechanicService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public MechanicService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    
}