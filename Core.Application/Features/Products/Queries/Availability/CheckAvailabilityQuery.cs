using MediatR;

namespace Core.Application.Features.Products.Queries.Availability
{
    public class CheckAvailabilityQuery : IRequest<CheckAvailabilityResult>
    {
        public Guid ProductId { get; set; }
        public CheckAvailabilityQuery(Guid productId)
        {
            ProductId = productId;
        }

    }
}
