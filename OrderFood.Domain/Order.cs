using System;
using System.Collections.Generic;
using OrderFood.Domain.Base;

namespace OrderFood.Domain
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
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
        public EnumPaymentStatus PaymentStatus { get; set; }
        public EnumOrderStatus OrderStatus { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public enum EnumPaymentMethod
    {
        Online = 0,
        Cash = 1,
        CreditCard = 2,
    }

    public enum EnumOrderStatus
    {
        Completed = 0,
        Preparing = 1,
        OutForDelivery = 2,
        Delivered = 3,
    }

    public enum EnumPaymentStatus
    {
        Unpaid = 0,
        Paid = 1,
    }
}
