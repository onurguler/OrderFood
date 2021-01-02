using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderFood.Web.Models
{
    public class CategoryModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public List<ProductModel> Products { get; set; }
    }
}
