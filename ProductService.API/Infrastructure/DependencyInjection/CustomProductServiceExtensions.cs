using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services.Concreate;
using Core.Application.Services.Factories;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Factories;
using Infrastructure.Persistence.Repositories.Concreate;

namespace ProductServiceAPI.Infrastructure.DependencyInjection
{
    public static class CustomProductServiceExtensions
    {
        public static IServiceCollection RegisterCustomProductInfrastructure(this IServiceCollection services)
        {
            //Repositories
            services.AddKeyedScoped<IProductRepository<CustomProduct>, CustomProductRepository>("custom");
            services.AddScoped<IProductRepository<CustomProduct>, CustomProductRepository>();

            // External API adapters
            //none

            //repository factory
            services.AddScoped<IProductRepositoryFactory>(sp =>
              new ProductRepositoryFactory<CustomProduct>(
                  sp.GetRequiredService<AppDbContext>(),
                  ProductCategory.Custom,
                  context => new CustomProductRepository(context))
              );

            return services;
        }

        public static IServiceCollection RegisterCustomProductServices(this IServiceCollection services)
        {
            //Service registration keyed and non-keyed
            services.AddKeyedScoped<IProductService<CustomProduct, CustomProductDto>, CustomProductService>("custom", (sp, _) =>
               new CustomProductService(
                   sp.GetRequiredKeyedService<IProductRepository<CustomProduct>>("custom")
               ));

            services.AddScoped(sp =>
                sp.GetRequiredKeyedService<IProductService<CustomProduct, CustomProductDto>>("custom"));

            // Service factory
            services.AddScoped<IProductServiceFactory>(sp =>
                new ProductServiceFactory<CustomProduct, CustomProductDto>(
                    sp,
                    ProductCategory.Custom,
                    "custom",
                    (repo, _) => new CustomProductService(repo)));

            // Additional special registrations for editable and addable products
            services.AddScoped(sp =>
                (IEditableProduct<CustomProductDto>)sp.GetRequiredKeyedService<IProductService<CustomProduct, CustomProductDto>>("custom"));

            services.AddScoped(sp =>
               (IAddableProduct<CustomProduct, CustomProductDto>)sp.GetRequiredKeyedService<IProductService<CustomProduct, CustomProductDto>>("custom"));

            return services;
        }
    }
}