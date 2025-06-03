using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Products.Commands.PurchaseProduct
{
    public class PurchaseProductCommand : IRequest<PurchaseResponseDto>
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public PurchaseProductCommand(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
        public PurchaseProductCommand()
        {

        }
    }
}
