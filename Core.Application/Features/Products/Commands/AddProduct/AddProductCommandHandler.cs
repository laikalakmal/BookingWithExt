using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using Core.Domain.ValueObjects;
using MediatR;

namespace Core.Application.Features.Products.Commands.AddProduct
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Guid>
    {
        private readonly IAddableProduct _customProductService;

        public AddProductCommandHandler(IAddableProduct customProductService)
        {
            _customProductService = customProductService ?? throw new ArgumentNullException(nameof(customProductService));
        }

        public async Task<Guid> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            AddProductRequest productRequest = request.Request;

            // Map to DTO
            var customProductDto = new ProductDto(
                id: Guid.NewGuid(),
                externalId: productRequest.ExternalId ?? Guid.NewGuid().ToString(),
                name: productRequest.Name ?? string.Empty,
                price: Price.Create(
                    productRequest.Amount ?? 0,
                    productRequest.Currency ?? Currency.USD.ToString()
                ),
                availability: new AvailabilityInfo
                {
                    Status = productRequest.Availability?.Status,
                    RemainingSlots = productRequest.Availability?.RemainingSlots ?? 0
                },
                description: productRequest.Description ?? string.Empty,
                category: productRequest.Category,
                provider: productRequest.Provider ?? "BookWithExt",
                imageUrl: new List<string> { productRequest.ImageUrl ?? "" } ?? [],
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow
            )
            {
                Attributes = productRequest.Attributes ?? []
            };

            // Use the service to add the product
            var result = await _customProductService.AddProductAsync(customProductDto);

            return result;
        }
    }
}


