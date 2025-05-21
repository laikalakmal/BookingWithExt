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
            var holidayCount = await _holidayService.SyncProductsFromExternalAsync();
            var tourCount = await _tourService.SyncProductsFromExternalAsync();

            return new SyncProductsResponse
            {
                HolidayCount = holidayCount,
                TourCount = tourCount
            };
        }
    }
}