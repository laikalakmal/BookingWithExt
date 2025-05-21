using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Products.Queries.GetHolidayPackages
{
    public class GetHolidayPackagesQueryHandler : IRequestHandler<GetHolidayPackagesQuery, List<HolidayPackageDto>>
    {
        private readonly IProductService<HolidayPackage, HolidayPackageDto> _holidayService;

        public GetHolidayPackagesQueryHandler(IProductService<HolidayPackage, HolidayPackageDto> holidayService)
        {
            _holidayService = holidayService;
        }

        public async Task<List<HolidayPackageDto>> Handle(GetHolidayPackagesQuery request, CancellationToken cancellationToken)
        {
            var products = await _holidayService.GetProductsAsync();
            return products.Where(p => p.Category == ProductCategory.HolidayPackage).ToList();
        }
    }
}