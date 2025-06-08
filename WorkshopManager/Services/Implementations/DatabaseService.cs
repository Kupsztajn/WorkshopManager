using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services.Interfaces;

namespace WorkshopManager.Services.Implementations;

public class DatabaseService : IDatabaseService
{
    private readonly UsersDbContext _context;

    public DatabaseService(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vehicle>> GetVehiclesAsync()
    {
        return await _context.Vehicles.ToListAsync();
    }

    public async Task<List<ServiceOrder>> GetServiceOrdersAsync()
    {
        return await _context.ServiceOrders.ToListAsync();
    }
}