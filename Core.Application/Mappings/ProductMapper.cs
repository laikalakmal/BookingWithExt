using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;


namespace Core.Application.Mappings
{
    public class ProductMapper : IProductMapper
    {
        public static ProductDto FromDomain(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");

            return new ProductDto(
                    id: product.Id,
                    externalId: product.ExternalId ?? "",
                    name: product.Name??"",
                    price: product.Price??Price.Create(0,""),
                    availability: product.Availability,
                    description: product.Description ?? "",
                    category: product.Category,
                    provider: product.Provider,
                    imageUrl: product.ImageUrl,
                    createdAt: product.CreatedAt,
                    updatedAt: product.UpdatedAt
                )
            {
                Attributes = product.Attributes,
                ImageUrl = product.ImageUrl ?? []
            };
        }

        public static Product ToDomain(ProductDto productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException(nameof(productDto), "ProductDto cannot be null.");

            return new Product(
                externalId: productDto.ExternalId,
                name: productDto.Name,
                price: productDto.Price,
                description: productDto.Description,
                category: productDto.Category,
                provider: productDto.Provider,
                availability: productDto.Availability
            )
            {
                Id = productDto.Id,
                CreatedAt = productDto.CreatedAt,
                UpdatedAt = productDto.UpdatedAt,
                ImageUrl = productDto.ImageUrl ?? [],
                Attributes = productDto.Attributes
            };
        }
    }
}