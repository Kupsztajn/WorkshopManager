using System.Threading.Tasks;
using WorkshopManager.Models;
namespace WorkshopManager.Services
{
    public interface IUserService
    {
        Task<User?> Authenticate(string username, string password);

    }
}
