using System.ComponentModel.DataAnnotations;

namespace OrderFood.Web.Models
{
    public class RoleModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
    }
}
