using MediatR;
using ProductService.API.Controllers;

namespace Core.Application.Features.Products.Queries.Availability
{
    internal class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, CheckAvailabilityResult>
    {

        private readonly IMediator _mediator;

        public CheckAvailabilityQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<CheckAvailabilityResult> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(request.ProductId), cancellationToken);
            if (product == null)
            {
                return new CheckAvailabilityResult
                {
                    Status = "NotFound"
                };
            }

            return new CheckAvailabilityResult
            {
                CurrentPrice = product.Price.Amount,
                Status = product.Availability.Status,
                IsAvailable = product.Availability.IsAvailable,
                RemainingSlots = product.Availability.RemainingSlots > 0 ? product.Availability.RemainingSlots : 0
            };
        }
    }
}
