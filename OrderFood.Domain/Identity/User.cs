using Microsoft.AspNetCore.Identity;

namespace OrderFood.Domain.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
