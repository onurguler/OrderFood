using System.Collections.Generic;
using OrderFood.Domain.Base;

namespace OrderFood.Domain
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
