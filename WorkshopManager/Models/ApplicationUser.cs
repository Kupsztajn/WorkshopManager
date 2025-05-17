using Microsoft.AspNetCore.Identity;

namespace WorkshopManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        // Możesz usunąć Login i PasswordHash, bo IdentityUser już ma UserName i PasswordHash obsługiwane.
    }
}