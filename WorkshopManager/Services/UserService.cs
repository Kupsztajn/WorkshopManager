using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;

namespace WorkshopManager.Services
{
    public class UserService : IUserService
    {
        private UsersDbContext _usersDbContext;

        public UserService(UsersDbContext usersDbContext)
        {
            _usersDbContext = usersDbContext;
        }

        public Task<User?> Authenticate(string login, string password)
        {
            return _usersDbContext.Users.FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == password);
        }
    }
}
