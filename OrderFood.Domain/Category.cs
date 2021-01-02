using System.Collections.Generic;
using OrderFood.Domain.Base;

namespace OrderFood.Domain
{
    public class Category : BaseEntity
    {
        public string Title { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
