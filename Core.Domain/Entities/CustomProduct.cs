using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Domain.Entities
{
    public class CustomProduct : Product
    {
        public CustomProduct()
        {
            Attributes = [];
        }
        public CustomProduct(string externalId,
                             string name,
                             Price price,
                             string description,
                             ProductCategory category,
                             string provider,
                             AvailabilityInfo availability,
                             Dictionary<string, object> attributes
            ) : base(externalId, name, price, description, category, provider, availability)
        {
            Attributes = attributes ?? [];
        }

        public Dictionary<string, object> Attributes { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
