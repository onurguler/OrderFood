using System.Collections.Generic;
using OrderFood.Domain.Models.Base;

namespace OrderFood.Domain.Models
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
