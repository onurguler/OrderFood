namespace OrderFood.Web.Models
{
    public class CartItemModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
