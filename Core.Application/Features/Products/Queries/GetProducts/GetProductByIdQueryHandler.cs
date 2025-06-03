using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;
using ProductService.API.Controllers;

namespace Core.Application.Features.Products.Queries.GetProducts
{
    internal class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository<Product> _productRepository;
        private readonly IProductService<Product, ProductDto> _productService;

        public GetProductByIdQueryHandler(
            IProductRepository<Product> productRepository,
            IProductService<Product, ProductDto> productService)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Product> products = await _productRepository.GetProductsAsync();
                Product? product = products.FirstOrDefault(p => p.Id == request.Id);

                return product == null
                    ? throw new KeyNotFoundException($"Product with ID {request.Id} not found.")
                    : _productService.MapToDto(product);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
