using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Products.Commands.EditCustomProducts
{
    public class EditProductCommand : IRequest<EditProductResponse>
    {

        public required AddProductRequest productRequest { get; set; }
        public required Guid ProductId { get; set; }

    }
}
