using System.Collections.Generic;
using OrderFood.Domain.Models.Base;

namespace OrderFood.Domain.Models
{
    public class Product : BaseEntity
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
