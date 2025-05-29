using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    /// <summary>
    /// Factory interface for creating product services based on product types.
    /// </summary>
    public interface IProductServiceFactory
    {
        /// <summary>
        /// Determines if this factory can handle the given product type.
        /// </summary>
        bool CanHandle(Product product);

        /// <summary>
        /// Creates the appropriate product service for a product.
        /// </summary>
        IProductService<Product, ProductDto> CreateService();
    }
}