using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Products.Commands.AddProduct
{
    public class AddProductCommand : IRequest<Guid>
    {
        public AddProductRequest Request { get; set; }

        public AddProductCommand(AddProductRequest request)
        {
            this.Request = request;
        }
    }
}