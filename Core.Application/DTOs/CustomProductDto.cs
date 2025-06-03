

using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Application.DTOs
{
    public class CustomProductDto : ProductDto
    {
        public CustomProductDto(Guid id, string externalId, string name, Price price, AvailabilityInfo availability, string description, ProductCategory category, string provider, string imageUrl, DateTime createdAt, DateTime updatedAt, Dictionary<string, object> attributes) : base(id, externalId, name, price, availability, description, category, provider, imageUrl, createdAt, updatedAt)
        {
            Attributes = attributes ?? [];
        }

        public Dictionary<string, object> Attributes { get; set; }
       


    }
}
