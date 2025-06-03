using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Products.Queries.GetAllProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {

        private readonly IProductService<TourPackage, TourPackageDto> _tourPackageService;
        private readonly IProductService<HolidayPackage, HolidayPackageDto> _holidayPackageService;
        private readonly IProductService<CustomProduct, CustomProductDto> _customProductService;

        public GetProductsQueryHandler(
            IProductService<TourPackage, TourPackageDto> tourPackageService,
            IProductService<HolidayPackage, HolidayPackageDto> holidayPackageService,
            IProductService<CustomProduct, CustomProductDto> customProductService)
        {
            _tourPackageService = tourPackageService;
            _holidayPackageService = holidayPackageService;
            _customProductService = customProductService;
        }

        async Task<List<ProductDto>> IRequestHandler<GetProductsQuery, List<ProductDto>>.Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var allProducts = new List<ProductDto>();

            try
            {
                var holidayPackages = new List<HolidayPackageDto>();
                var tourPackages = new List<TourPackageDto>();
                var customProducts = new List<CustomProductDto>();

                switch (request.Category)
                {
                    case Domain.Enums.ProductCategory.HolidayPackage:

                        holidayPackages = await _holidayPackageService.GetProductsAsync() as List<HolidayPackageDto>
                            ?? [];

                        // Apply filters
                        if (request.Provider != null && request.Provider != string.Empty)
                        {
                            holidayPackages = holidayPackages.Where(p => p.Provider == request.Provider).ToList();
                        }
                        if (request.ExternalId != null && request.ExternalId != string.Empty)
                        {
                            holidayPackages = holidayPackages.Where(p => p.ExternalId == request.ExternalId).ToList();
                        }

                        allProducts.AddRange(holidayPackages.Select(p => (ProductDto)p));
                        break;

                    case Domain.Enums.ProductCategory.TourPackage:
                        tourPackages = await _tourPackageService.GetProductsAsync() as List<TourPackageDto>
                            ?? [];

                        // Apply filters
                        if (request.Provider != null && request.Provider != string.Empty)
                        {
                            tourPackages = tourPackages.Where(p => p.Provider == request.Provider).ToList();
                        }
                        if (request.ExternalId != null && request.ExternalId != string.Empty)
                        {
                            tourPackages = tourPackages.Where(p => p.ExternalId == request.ExternalId).ToList();
                        }

                        allProducts.AddRange(tourPackages.Select(p => (ProductDto)p));
                        break;

                    case Domain.Enums.ProductCategory.Custom:
                        customProducts = await _customProductService.GetProductsAsync() as List<CustomProductDto>
                            ?? [];
                        // Apply filters
                        if (request.Provider != null && request.Provider != string.Empty)
                        {
                            customProducts = customProducts.Where(p => p.Provider == request.Provider).ToList();
                        }
                        if (request.ExternalId != null && request.ExternalId != string.Empty)
                        {
                            customProducts = customProducts.Where(p => p.ExternalId == request.ExternalId).ToList();
                        }
                        allProducts.AddRange(customProducts.Select(p => (ProductDto)p));
                        break;

                    default:
                        // Get all types of products when no category is specified
                        holidayPackages = await _holidayPackageService.GetProductsAsync() as List<HolidayPackageDto>
                            ?? [];
                        tourPackages = await _tourPackageService.GetProductsAsync() as List<TourPackageDto>
                            ?? [];
                        customProducts = await _customProductService.GetProductsAsync() as List<CustomProductDto>
                            ?? [];

                        // Apply filters to both collections
                        if (request.Provider != null && request.Provider != string.Empty)
                        {
                            holidayPackages = holidayPackages.Where(p => p.Provider == request.Provider).ToList();
                            tourPackages = tourPackages.Where(p => p.Provider == request.Provider).ToList();
                            customProducts = customProducts.Where(p => p.Provider == request.Provider).ToList();
                        }
                        if (request.ExternalId != null && request.ExternalId != string.Empty)
                        {
                            holidayPackages = holidayPackages.Where(p => p.ExternalId == request.ExternalId).ToList();
                            tourPackages = tourPackages.Where(p => p.ExternalId == request.ExternalId).ToList();
                        }

                        allProducts.AddRange(holidayPackages.Select(p => (ProductDto)p));
                        allProducts.AddRange(tourPackages.Select(p => (ProductDto)p));
                        allProducts.AddRange(customProducts.Select(p => (ProductDto)p));
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return allProducts;
        }
    }
}
