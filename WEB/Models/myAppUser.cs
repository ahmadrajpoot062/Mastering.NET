using Microsoft.AspNetCore.Identity;

namespace Mastering.NET.Models
{
    public class myAppUser : IdentityUser
    {
        public string FirstName { get;set; }
        public string LastName { get;set; }
        
    }
}
