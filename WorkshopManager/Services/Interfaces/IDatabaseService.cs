using WorkshopManager.Models;

namespace WorkshopManager.Services.Interfaces;

public interface IDatabaseService
{
    public Task<List<Vehicle>> GetVehiclesAsync();
    public Task<List<ServiceOrder>> GetServiceOrdersAsync();
}