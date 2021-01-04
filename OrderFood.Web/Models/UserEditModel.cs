using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderFood.Web.Models
{
    public class UserEditModel
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public IList<string> UserRoles { get; set; }
        public List<string> AvailableRoles { get; set; }
    }
}
