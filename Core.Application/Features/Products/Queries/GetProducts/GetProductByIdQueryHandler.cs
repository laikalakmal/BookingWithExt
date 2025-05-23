using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Domain.Entities;
using MediatR;
using ProductService.API.Controllers;

namespace Core.Application.Features.Products.Queries.GetProducts
{
    internal class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository<Product> _productRepository;
        private readonly IEnumerable<IProductServiceFactory> _serviceFactories;

        public GetProductByIdQueryHandler(IProductRepository<Product> productRepository, IEnumerable<IProductServiceFactory> serviceFactories)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));
        }
        
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Product> products = await _productRepository.GetProductsAsync();
            Product? product = products.FirstOrDefault(p => p.Id == request.Id);
            
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.Id} not found.");
            }
            
            // Use a service that can determine the right mapper
            var productService = new GenericProductService(_serviceFactories, _productRepository);
            return productService.MapToDto(product);
        }
    }
}
