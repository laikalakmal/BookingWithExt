namespace Core.Application.DTOs
{
    public class PurchaseRequest
    {
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}
