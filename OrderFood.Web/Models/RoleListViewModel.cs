using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace OrderFood.Web.Models
{
    public class RoleListViewModel
    {
        public List<IdentityRole> Roles { get; set; }
    }
}
