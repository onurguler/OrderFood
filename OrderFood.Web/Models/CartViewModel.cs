using System.Collections.Generic;
using System.Linq;

namespace OrderFood.Web.Models
{
    public class CartViewModel
    {
        public long Id { get; set; }
        public List<CartItemModel> CartItems { get; set; }

        public decimal TotalPrice()
        {
            return CartItems.Sum(item => item.Price * item.Quantity);
        }
    }
}
