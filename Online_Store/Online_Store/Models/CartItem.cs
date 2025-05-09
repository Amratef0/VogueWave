namespace Online_Store.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; } = 1; // default 1

    }

}
