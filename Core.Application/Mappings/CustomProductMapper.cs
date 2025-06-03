using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;


namespace Core.Application.Mappings
{
    public class CustomProductMapper : IProductMapper<CustomProduct, CustomProductDto>
    {
        public static CustomProductDto FromDomain(CustomProduct product)
        {
            if (product == null)
                return null;

            return new CustomProductDto(
                    id: product.Id,
                    externalId: product.ExternalId ?? "",
                    name: product.Name,
                    price: product.Price,
                    availability: product.Availability,
                    description: product.Description ?? "",
                    category: product.Category,
                    provider: product.Provider,
                    imageUrl: product.ImageUrl,
                    createdAt: product.CreatedAt,
                    updatedAt: product.UpdatedAt,
                    attributes: product.Attributes
                )
            {
                Id = product.Id,
                ExternalId = product.ExternalId ?? "",
                Name = product.Name,
                Price = product.Price,
                Description = product.Description ?? "",
                Category = product.Category,
                Provider = product.Provider,
                Availability = product.Availability,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Attributes = product.Attributes,
                ImageUrl= product.ImageUrl ?? string.Empty
            };
        }

        public static CustomProduct ToDomain(CustomProductDto productDto)
        {
            if (productDto == null)
                return null;

            return new CustomProduct(
                externalId: productDto.ExternalId,
                name: productDto.Name,
                price: productDto.Price,
                description: productDto.Description,
                category: productDto.Category,
                provider: productDto.Provider,
                availability: productDto.Availability,
                attributes: productDto.Attributes
            )
            {
                Id = productDto.Id,
                CreatedAt = productDto.CreatedAt,
                UpdatedAt = productDto.UpdatedAt,
                ImageUrl = productDto.ImageUrl ?? string.Empty
            };
        }
    }
}