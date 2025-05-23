using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services.Concreate;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Services.Factories
{
    public class HolidayPackageServiceFactory : IProductServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public HolidayPackageServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool CanHandle(Product product)
        {
            return product.Category == ProductCategory.HolidayPackage;
        }

        public IProductService<Product, ProductDto> CreateService()
        {
            var repository = _serviceProvider.GetKeyedService<IProductRepository<HolidayPackage>>("holiday");
            var adapter = _serviceProvider.GetKeyedService<IExternalProductApiAdapter>("holiday");

            if (repository == null)
            {
                throw new InvalidOperationException("The required repository service is not registered or resolved.");
            }

            if (adapter == null)
            {
                throw new InvalidOperationException("The required external product API adapter service is not registered or resolved.");
            }

            return (IProductService<Product, ProductDto>)(object)new HolidayPackageService(repository, adapter);
        }
    }
}
