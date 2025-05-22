using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Products.Queries.GetTourPackages
{
    public class GetTourPackagesQueryHandler : IRequestHandler<GetTourPackagesQuery, List<TourPackageDto>>
    {
        private readonly IProductService<TourPackage, TourPackageDto> _tourService;

        public GetTourPackagesQueryHandler(IProductService<TourPackage, TourPackageDto> tourService)
        {
            _tourService = tourService;
        }

        public async Task<List<TourPackageDto>> Handle(GetTourPackagesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _tourService.GetProductsAsync();
                return products.Where(p => p.Category == ProductCategory.TourPackage).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

