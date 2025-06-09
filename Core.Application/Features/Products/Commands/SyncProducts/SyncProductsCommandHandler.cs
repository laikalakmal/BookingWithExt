using Core.Application.Interfaces;
using MediatR;

namespace Core.Application.Features.Products.Commands.SyncProducts
{
    public class SyncProductsCommandHandler : IRequestHandler<SyncProductsCommand, SyncProductsResponse>
    {
        private readonly IProductService _service;

        public SyncProductsCommandHandler(IProductService service)
        {
            _service = service;
        }

        public async Task<SyncProductsResponse> Handle(SyncProductsCommand request, CancellationToken cancellationToken)
        {
            var response = new SyncProductsResponse();
            try
            {
                var holidayCount = await _service.SyncProductsFromExternalAsync();
                response.HolidayCount = holidayCount;

            }
            catch (Exception ex)
            {
                response.errorMessages.Add("Error syncing products: " + ex.Message);

            }



            return response;



        }
    }
}