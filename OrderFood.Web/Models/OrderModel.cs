using System;
using System.Collections.Generic;
using OrderFood.Domain;

namespace OrderFood.Web.Models
{
    public class OrderModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ForDirections { get; set; }
        public string Note { get; set; }
        public decimal Price { get; set; }
        public DateTime DateOrdered { get; set; }
        public EnumPaymentMethod PaymentMethod { get; set; }
        public List<OrderItemModel> OrderItems { get; set; }
    }
}
