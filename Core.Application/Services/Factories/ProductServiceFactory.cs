using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Services.Factories
{
    public class ProductServiceFactory<TEntity, TDto> : IProductServiceFactory
        where TEntity : Product
        where TDto : ProductDto
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ProductCategory _supportedCategory;
        private readonly string _keyedServiceName;
        private readonly Func<IProductRepository<TEntity>, IExternalProductApiAdapter, IProductService<TEntity, TDto>> _serviceFactory;

        public ProductServiceFactory(
            IServiceProvider serviceProvider,
            ProductCategory supportedCategory,
            string keyedServiceName,
            Func<IProductRepository<TEntity>, IExternalProductApiAdapter, IProductService<TEntity, TDto>> serviceFactory)
        {
            _serviceProvider = serviceProvider;
            _supportedCategory = supportedCategory;
            _keyedServiceName = keyedServiceName;
            _serviceFactory = serviceFactory;
        }

        public bool CanHandle(Product product)
        {
            return product.Category == _supportedCategory;
        }

        public IProductService<Product, ProductDto> CreateService()
        {
            var repository = _serviceProvider.GetKeyedService<IProductRepository<TEntity>>(_keyedServiceName) 
                ?? throw new InvalidOperationException($"{typeof(TEntity).Name} repository service not found");
            
            var adapter = _serviceProvider.GetKeyedService<IExternalProductApiAdapter>(_keyedServiceName)
                ?? throw new InvalidOperationException($"{typeof(TEntity).Name} API adapter service not found");
            
            // Create the specific service
            var service = _serviceFactory(repository, adapter);
            
            // Wrap it in the adapter
            return new ProductServiceAdapter<TEntity, TDto>(service);
        }
    }
}