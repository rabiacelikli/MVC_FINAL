using Microsoft.AspNetCore.Identity;

namespace uyg1.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }

    }
}
