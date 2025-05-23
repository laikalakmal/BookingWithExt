using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Products.Commands.SyncProducts
{
    public class SyncProductsCommandHandler : IRequestHandler<SyncProductsCommand, SyncProductsResponse>
    {
        private readonly IProductService<HolidayPackage, HolidayPackageDto> _holidayService;
        private readonly IProductService<TourPackage, TourPackageDto> _tourService;

        public SyncProductsCommandHandler(
            IProductService<HolidayPackage, HolidayPackageDto> holidayService,
            IProductService<TourPackage, TourPackageDto> tourService)
        {
            _holidayService = holidayService;
            _tourService = tourService;
        }

        public async Task<SyncProductsResponse> Handle(SyncProductsCommand request, CancellationToken cancellationToken)
        {
            var response = new SyncProductsResponse();
            try
            {
                var holidayCount = await _holidayService.SyncProductsFromExternalAsync();
                response.HolidayCount = holidayCount;

            }
            catch (Exception ex)
            {
                response.errorMessages.Add("Error syncing holiday products: " + ex.Message);

            }

            try
            {
                var tourCount = await _tourService.SyncProductsFromExternalAsync();
                response.TourCount = tourCount;

            }
            catch (Exception ex)
            {

                response.errorMessages.Add("Error syncing tour products: " + ex.Message);
            }

            return response;



        }
    }
}