using OrderFood.Domain.Base;

namespace OrderFood.Domain
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
