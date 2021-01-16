using System.Collections.Generic;
using OrderFood.Domain.Models.Base;

namespace OrderFood.Domain.Models
{
    public class Category : BaseEntity
    {
        public string Title { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
