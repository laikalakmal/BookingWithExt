namespace Core.Application.Features.Products.Queries.Availability
{
    public class CheckAvailabilityResult
    {
        public bool IsAvailable { get; set; }
        public string? Status { get; set; }
        public int RemainingSlots { get; set; }

        public decimal CurrentPrice { get; set; }
    }
}