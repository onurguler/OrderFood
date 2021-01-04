using System.Collections.Generic;
using OrderFood.Domain.Base;

namespace OrderFood.Domain
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
