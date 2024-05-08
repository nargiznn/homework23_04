using Microsoft.AspNetCore.Identity;

namespace Pustok.Models
{
    public class AppMember:IdentityUser
    {
        public string Email { get; set; }
    }
}
