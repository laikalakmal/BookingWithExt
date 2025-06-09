using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Products.Queries.GetAllProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IProductService _productService;

        public GetProductsQueryHandler(
            IProductService ProductService)
        {
            _productService = ProductService;
        }

        async Task<List<ProductDto>> IRequestHandler<GetProductsQuery, List<ProductDto>>.Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var allProducts = new List<ProductDto>();

            try
            {
                var customProducts = await _productService.GetProductsAsync() as List<ProductDto> ?? [];

                // Apply filters if provided
                if (!string.IsNullOrEmpty(request.Provider))
                {
                    customProducts = customProducts.Where(p => p.Provider == request.Provider).ToList();
                }

                if (!string.IsNullOrEmpty(request.ExternalId))
                {
                    customProducts = customProducts.Where(p => p.ExternalId == request.ExternalId).ToList();
                }

                // Filter by category if specified
                if (request.Category.HasValue && request.Category.Value != Domain.Enums.ProductCategory.Custom)
                {
                    // Return empty list if looking for non-custom products
                    return allProducts;
                }

                allProducts.AddRange(customProducts.Select(p => (ProductDto)p));
            }
            catch (Exception)
            {
                throw;
            }

            return allProducts;
        }
    }
}
