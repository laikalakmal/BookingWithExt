using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Products.Commands.AddProduct
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Guid>
    {
        private readonly IAddableProduct<CustomProduct,CustomProductDto> _customProductService;

        public AddProductCommandHandler(IAddableProduct<CustomProduct,CustomProductDto> customProductService)
        {
            _customProductService = customProductService ?? throw new ArgumentNullException(nameof(customProductService));
        }

        public async Task<Guid> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var productRequest = request.Request;

            // Map to DTO
            var customProductDto = new CustomProductDto(
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
                imageUrl: productRequest.ImageUrl ?? string.Empty,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                attributes: productRequest.Attributes ?? new Dictionary<string, object>()
            );

            // Use the service to add the product
            var result = await _customProductService.AddProductAsync(customProductDto);

            return result;
        }
    }
}


