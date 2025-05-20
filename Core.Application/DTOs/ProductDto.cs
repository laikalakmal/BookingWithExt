using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public Price Price { get; set; }
        public string Description { get; set; }
        public ProductCategory Category { get; set; }
        public string Provider { get; set; }
        
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        

        public ProductDto(
            Guid id,
            string externalId,
            string name,
            Price price,
            string description,
            ProductCategory category,
            string provider,
            string imageUrl,
            DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            ExternalId = externalId;
            Name = name;
            Price = price;
            Description = description;
            Category = category;
            Provider = provider;
            ImageUrl = imageUrl;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
