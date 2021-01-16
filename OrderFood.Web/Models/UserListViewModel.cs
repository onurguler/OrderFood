using System.Collections.Generic;
using OrderFood.Domain.Identity.Models;

namespace OrderFood.Web.Models
{
    public class UserListViewModel
    {
        public List<ApplicationUser> Users { get; set; }
    }
}
