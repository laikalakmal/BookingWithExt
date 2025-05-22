using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Products.Queries.GetHolidayPackages
{
    public class GetHolidayPackagesQueryHandler : IRequestHandler<GetHolidayPackagesQuery, List<HolidayPackageDto>>
    {
        private readonly IProductService<HolidayPackage, HolidayPackageDto> _holidayService;
        

        public GetHolidayPackagesQueryHandler(
            IProductService<HolidayPackage, HolidayPackageDto> holidayService)
        {
            _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
            
        }

        public async Task<List<HolidayPackageDto>> Handle(GetHolidayPackagesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                
                var products = await _holidayService.GetProductsAsync();
                return products.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}