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
    public static class HolidayPackageServiceExtensions
    {
        public static IServiceCollection RegisterHolidayPackageInfrastructure(this IServiceCollection services)
        {
            // REPOSITORIES
            services.AddKeyedScoped<IProductRepository<HolidayPackage>, HolidayPackageRepository>("holiday");
            services.AddScoped<IProductRepository<HolidayPackage>, HolidayPackageRepository>();

            // External API adapters
            services.AddKeyedScoped<IExternalProductApiAdapter, HolidayPackageAdapter>("holiday");
            services.AddScoped<IExternalProductApiAdapter, HolidayPackageAdapter>();

            // Repository factory
            services.AddScoped<IProductRepositoryFactory>(sp =>
                new ProductRepositoryFactory<HolidayPackage>(
                    sp.GetRequiredService<AppDbContext>(),
                    ProductCategory.HolidayPackage,
                    context => new HolidayPackageRepository(context))
            );

            return services;
        }

        public static IServiceCollection RegisterHolidayPackageServices(this IServiceCollection services)
        {
            // Keyed registration
            services.AddKeyedScoped<IProductService<HolidayPackage, HolidayPackageDto>, HolidayPackageService>("holiday", (sp, _) =>
                new HolidayPackageService(
                    sp.GetRequiredKeyedService<IProductRepository<HolidayPackage>>("holiday"),
                    sp.GetRequiredKeyedService<IExternalProductApiAdapter>("holiday")
                ));

            // Standard registration (for MediatR)
            services.AddScoped(sp =>
                sp.GetRequiredKeyedService<IProductService<HolidayPackage, HolidayPackageDto>>("holiday"));

            // Service factory
            services.AddScoped<IProductServiceFactory>(sp =>
                new ProductServiceFactory<HolidayPackage, HolidayPackageDto>(
                    sp,
                    ProductCategory.HolidayPackage,
                    "holiday",
                    (repo, adapter) => new HolidayPackageService(repo, adapter)));

            return services;
        }
    }
}