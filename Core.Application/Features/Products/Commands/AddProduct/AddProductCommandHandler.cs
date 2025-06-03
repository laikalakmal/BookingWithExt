using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Features.Products.Commands.AddProduct
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Guid>
    {
        private readonly IProductRepository<CustomProduct> _repository;

        public AddProductCommandHandler([FromKeyedServices("custom")] IProductRepository<CustomProduct> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var productRequest = request.Request;

            // Create price value object
            var price = Price.Create(productRequest.Amount ?? 0, productRequest.Currency ?? Currency.USD.ToString());

            // Create custom product entity
            var customProduct = new CustomProduct(
                externalId: productRequest.ExternalId ?? Guid.NewGuid().ToString(),
                name: productRequest.Name ?? "",
                price: price,
                description: productRequest.Description ?? string.Empty,
                category: productRequest.Category,
                provider: productRequest.Provider ?? "BookWithExt",
                availability: productRequest.Availability,
                attributes: productRequest.Attributes
            );

            // Add product to database
            await _repository.AddProductsAsync(new List<CustomProduct> { customProduct });

            return customProduct.Id;
        }
    }
}