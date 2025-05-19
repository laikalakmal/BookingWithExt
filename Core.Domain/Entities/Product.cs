using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public abstract class Product
    {
        public Product(
            string externalId,
            string name,
            decimal price,
            string description,
            string category,
            string provider,
            string duration,
            string imageUrl)
        {
            Id = Guid.NewGuid();
            this.externalId = externalId;
            Name = name;
            Price = price;
            Description = description;
            Category = category;
            Provider = provider;
            Duration = duration;
            this.imageUrl = imageUrl;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public string externalId { get; set; } // what is the external id of the product
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } //defines is it an tour or a hotel
        public string Provider { get; set; } // what is the external provider of the product

        public string Duration { get;set; } 
        public string imageUrl { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        

      
       
    }
}
