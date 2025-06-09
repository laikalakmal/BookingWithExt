using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities.SupportClasses;
using MediatR;


namespace Core.Application.Features.Products.Commands.EditCustomProducts
{
    public class EditProductCommandHandler : IRequestHandler<EditProductCommand, EditProductResponse>
    {
        private readonly IEditableProduct _editableProduct;

        public EditProductCommandHandler(IEditableProduct editableProduct)
        {
            _editableProduct = editableProduct ?? throw new ArgumentNullException(nameof(editableProduct));
        }

        public async Task<EditProductResponse> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null || request.productRequest == null)
                {
                    return new EditProductResponse
                    {
                        Success = false,
                        Message = "Invalid request data."
                    };
                }

                // Map from AddProductRequest to CustomProductDto
                var price = Price.Create(
                    request.productRequest.Amount ?? 0,
                    request.productRequest.Currency ?? "USD");

                var availability = new AvailabilityInfo(
                    request.productRequest.Availability?.Status,
                    request.productRequest.Availability?.RemainingSlots ?? 0);

                var customProductDto = new ProductDto(
                    id: request.ProductId,
                    externalId: request.productRequest.ExternalId ?? string.Empty,
                    name: request.productRequest.Name ?? string.Empty,
                    price: price,
                    availability: availability,
                    description: request.productRequest.Description ?? string.Empty,
                    category: request.productRequest.Category,
                    provider: request.productRequest.Provider ?? string.Empty,
                    imageUrl: request.productRequest.ImageUrl ?? string.Empty,
                    createdAt: DateTime.UtcNow,
                    updatedAt: DateTime.UtcNow
                )
                {
                    Attributes = request.productRequest.Attributes
                };

                // Call the editableProduct service to update the product
                var result = await _editableProduct.EditProduct(request.ProductId, customProductDto);

                return new EditProductResponse
                {
                    Success = result,
                    Message = result ? "Product updated successfully." : "Failed to update product."
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new EditProductResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new EditProductResponse
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }
    }

    public class EditProductResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
