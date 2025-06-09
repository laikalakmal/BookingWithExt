using Core.Application.Interfaces;
using Infrastructure.Adapters;
using Infrastructure.Persistence.Repositories;


namespace ProductServiceAPI.Infrastructure.DependencyInjection
{
    public static class ProductServiceExtensions
    {
        public static IServiceCollection RegisterProductInfrastructure(this IServiceCollection services)
        {
            //Repositories
            services.AddScoped<IProductRepository, ProductRepository>();

            //Adapters
            services.AddScoped<IExternalProductApiAdapter, TourApiAdapter>();
            services.AddScoped<IExternalProductApiAdapter, HolidayPackageAdapter>();


            return services;
        }

        public static IServiceCollection RegisterProductServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IProductService, Core.Application.Services.ProductService>();
            services.AddScoped<IEditableProduct, Core.Application.Services.ProductService>();
            services.AddScoped< IAddableProduct, Core.Application.Services.ProductService>();

            return services;
        }
    }
}