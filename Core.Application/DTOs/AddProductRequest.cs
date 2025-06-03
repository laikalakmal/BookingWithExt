using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Application.DTOs
{
    public class AddProductRequest
    {

        public string? Name { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; } = "USD";
        public string? Description { get; set; }
        public ProductCategory Category { get; set; }
        public string? Provider { get; set; }
        public string? ExternalId { get; set; }
        public AvailabilityInfo Availability { get; set; } = new AvailabilityInfo { Status = "Available", RemainingSlots = 10 };

        public string? ImageUrl { get; set; }

        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
    }
}
