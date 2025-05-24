using Core.Application.DTOs;
using MediatR;

namespace ProductService.API.Controllers
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}