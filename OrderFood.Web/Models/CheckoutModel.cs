using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OrderFood.Domain.Models;

namespace OrderFood.Web.Models
{
    public class CheckoutModel
    {

        [Required]
        [DisplayName("Delivery Address")]
        public string Address { get; set; }

        [DisplayName("For Directions")]
        public string ForDirections { get; set; }

        public string Note { get; set; }

        public EnumPaymentMethod PaymentMethod { get; set; }

        [Required]
        [DisplayName("Card Name")]
        public string CardName { get; set; }

        [CreditCard]
        [DisplayName("Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [DisplayName("Expiration Month")]
        public string ExpirationMonth { get; set; }

        [Required]
        [DisplayName("Expiration Year")]
        public string ExpirationYear { get; set; }

        [Required]
        public string CVC { get; set; }

        public CartViewModel Cart { get; set; }
    }
}
