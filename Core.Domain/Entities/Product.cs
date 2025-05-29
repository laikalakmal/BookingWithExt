
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Domain.Entities
{
    public abstract class Product
    {
        public Product(
            string externalId,
            string name,
            Price price,
            string description,
            ProductCategory category,
            string provider,
            AvailabilityInfo availability)
        {
            Id = Guid.NewGuid();
            this.ExternalId = externalId;
            Name = name;
            Price = price;
            Description = description;
            Category = category;
            Provider = provider;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            this.Availability = availability;
        }

        public Guid Id { get; set; }

        public string ExternalId { get; set; } // what is the external id of the product
        public string Name { get; set; }
        public Price Price { get; set; }
        public string? Description { get; set; }
        public ProductCategory Category { get; set; } //defines is it an tour or a hotel or something else
        public string Provider { get; set; } // what is the external provider of the product ex: booking.com

        

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AvailabilityInfo Availability { get ; set ; }
    }
}
