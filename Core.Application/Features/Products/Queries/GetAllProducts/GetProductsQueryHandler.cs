using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Features.Products.Queries.GetAllProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {

        private readonly IProductService<TourPackage, TourPackageDto> _tourPackageService;
        private readonly IProductService<HolidayPackage, HolidayPackageDto> _holidayPackageService;

        public GetProductsQueryHandler(
            IProductService<TourPackage, TourPackageDto> tourPackageService,
            IProductService<HolidayPackage, HolidayPackageDto> holidayPackageService)
        {
            _tourPackageService = tourPackageService;
            _holidayPackageService = holidayPackageService;
        }

        Task<List<ProductDto>> IRequestHandler<GetProductsQuery, List<ProductDto>>.Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var tourPackages = _tourPackageService.GetProductsAsync().Result;
            var holidayPackages = _holidayPackageService.GetProductsAsync().Result;
            var allProducts = new List<ProductDto>();
            allProducts.AddRange(tourPackages.Select(p => (ProductDto)p));
            allProducts.AddRange(holidayPackages.Select(p => (ProductDto)p));
            return Task.FromResult(allProducts);
        }
    }
}
