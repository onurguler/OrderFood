using System.Collections.Generic;
using OrderFood.Domain.Models.Base;

namespace OrderFood.Domain.Models
{
    public class CartItem : BaseEntity
    {
        public long CartId { get; set; }
        public Cart Cart { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
