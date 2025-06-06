using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services.Concreate;
using Core.Application.Services.Factories;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.Adapters;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Factories;
using Infrastructure.Persistence.Repositories.Concreate;

namespace ProductServiceAPI.Infrastructure.DependencyInjection
{
    public static class TourPackageServiceExtensions
    {
        public static IServiceCollection RegisterTourPackageInfrastructure(this IServiceCollection services)
        {
            // Repositories
            services.AddKeyedScoped<IProductRepository<TourPackage>, TourPackageRepository>("tour");
            services.AddScoped<IProductRepository<TourPackage>, TourPackageRepository>();

            // External API adapters
            services.AddKeyedScoped<IExternalProductApiAdapter, TourApiAdapter>("tour");
            services.AddScoped<IExternalProductApiAdapter, TourApiAdapter>();

            // Repository factory
            services.AddScoped<IProductRepositoryFactory>(sp =>
                new ProductRepositoryFactory<TourPackage>(
                    sp.GetRequiredService<AppDbContext>(),
                    ProductCategory.TourPackage,
                    context => new TourPackageRepository(context))
            );

            return services;
        }

        public static IServiceCollection RegisterTourPackageServices(this IServiceCollection services)
        {
            // Keyed registration
            services.AddKeyedScoped<IProductService<TourPackage, TourPackageDto>, TourPackageService>("tour", (sp, _) =>
                new TourPackageService(
                    sp.GetRequiredKeyedService<IProductRepository<TourPackage>>("tour"),
                    sp.GetRequiredKeyedService<IExternalProductApiAdapter>("tour")
                ));

            // Standard registration (for MediatR)
            services.AddScoped(sp =>
                sp.GetRequiredKeyedService<IProductService<TourPackage, TourPackageDto>>("tour"));

            // Service factory
            services.AddScoped<IProductServiceFactory>(sp =>
                new ProductServiceFactory<TourPackage, TourPackageDto>(
                    sp,
                    ProductCategory.TourPackage,
                    "tour",
                    (repo, adapter) => new TourPackageService(repo, adapter)));

            return services;
        }
    }
}