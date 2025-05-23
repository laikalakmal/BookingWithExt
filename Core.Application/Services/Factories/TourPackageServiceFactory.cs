using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services.Concreate;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Services.Factories
{
    public class TourPackageServiceFactory : IProductServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TourPackageServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool CanHandle(Product product)
        {
            return product.Category == ProductCategory.TourPackage;
        }

        public IProductService<Product, ProductDto> CreateService()
        {
            
            var repository = _serviceProvider.GetKeyedService<IProductRepository<TourPackage>>("tour") 
                ?? throw new InvalidOperationException("TourPackage repository service not found");
            
            var adapter = _serviceProvider.GetKeyedService<IExternalProductApiAdapter>("tour")
                ?? throw new InvalidOperationException("TourPackage API adapter service not found");
            
            // Cast needed because of variance limitations
            return (IProductService<Product, ProductDto>)(object)new TourPackageService(repository, adapter);
        }
    }
}