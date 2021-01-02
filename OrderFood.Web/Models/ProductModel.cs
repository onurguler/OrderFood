using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderFood.Web.Models
{
    public class ProductModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 1000000, ErrorMessage = "Price must be greater than 0.")]
        public decimal? Price { get; set; }
        public string ImageUrl { get; set; }
        public List<CategoryModel> SelectedCategories { get; set; }
    }
}
