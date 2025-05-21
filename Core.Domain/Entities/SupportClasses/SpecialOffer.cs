namespace Core.Domain.Entities.SupportClasses
{

    public class SpecialOffer
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Discount { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool RequiresVerification { get; set; }
        public decimal? PriceAddon { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int? MinimumNights { get; set; }
        public List<string> ValidFor { get; set; } = new List<string>();


    }
}
