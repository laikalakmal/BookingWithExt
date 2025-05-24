using Core.Application.DTOs;
using Core.Application.Features.Products.Queries.GetAllProducts;
using Core.Application.Features.Products.Queries.GetHolidayPackages;
using Core.Application.Features.Products.Queries.GetTourPackages;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Application.Services.Concreate;
using Core.Application.Services.Factories;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.Adapters;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Factories;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Concreate;
using Microsoft.EntityFrameworkCore;
// Extension methods for clean service registration by layer
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database context
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ProductDb")));

        // Repositories with keyed DI
        services.AddKeyedScoped<IProductRepository<TourPackage>, TourPackageRepository>("tour");
        services.AddKeyedScoped<IProductRepository<HolidayPackage>, HolidayPackageRepository>("holiday");

        // External API adapters
        services.AddKeyedScoped<IExternalProductApiAdapter, TourApiAdapter>("tour");
        services.AddKeyedScoped<IExternalProductApiAdapter, HolidayPackageAdapter>("holiday");

        // Standard repository registrations
        services.AddScoped<IProductRepository<TourPackage>, TourPackageRepository>();
        services.AddScoped<IProductRepository<HolidayPackage>, HolidayPackageRepository>();
        
      

        // Generic product repository (depends on repository factories)
        services.AddScoped<IProductRepository<Product>, GenericProductRepository>();


        services.AddScoped<IProductRepositoryFactory>(sp =>
          new ProductRepositoryFactory<TourPackage>(
        sp.GetRequiredService<AppDbContext>(),
        ProductCategory.TourPackage,
        context => new TourPackageRepository(context)));

        services.AddScoped<IProductRepositoryFactory>(sp =>
            new ProductRepositoryFactory<HolidayPackage>(
                sp.GetRequiredService<AppDbContext>(),
                ProductCategory.HolidayPackage,
                context => new HolidayPackageRepository(context)));

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register specific product services with keyed DI
        services.AddKeyedScoped<IProductService<TourPackage, TourPackageDto>, TourPackageService>("tour", (sp, _) =>
            new TourPackageService(
                sp.GetRequiredKeyedService<IProductRepository<TourPackage>>("tour"),
                sp.GetRequiredKeyedService<IExternalProductApiAdapter>("tour")
            ));

        services.AddKeyedScoped<IProductService<HolidayPackage, HolidayPackageDto>, HolidayPackageService>("holiday", (sp, _) =>
            new HolidayPackageService(
                sp.GetRequiredKeyedService<IProductRepository<HolidayPackage>>("holiday"),
                sp.GetRequiredKeyedService<IExternalProductApiAdapter>("holiday")
            ));

      

        // Register generic product service (depends on service factories)
        services.AddScoped<IProductService<Product, ProductDto>, GenericProductService>();

        services.AddScoped<IProductServiceFactory>(sp =>
    new ProductServiceFactory<TourPackage, TourPackageDto>(
        sp,
        ProductCategory.TourPackage,
        "tour",
        (repo, adapter) => new TourPackageService(repo, adapter)));

        services.AddScoped<IProductServiceFactory>(sp =>
            new ProductServiceFactory<HolidayPackage, HolidayPackageDto>(
                sp,
                ProductCategory.HolidayPackage,
                "holiday",
                (repo, adapter) => new HolidayPackageService(repo, adapter)));

        // Non-keyed registrations for MediatR
        services.AddScoped(sp =>
            sp.GetRequiredKeyedService<IProductService<TourPackage, TourPackageDto>>("tour"));

        services.AddScoped(sp =>
            sp.GetRequiredKeyedService<IProductService<HolidayPackage, HolidayPackageDto>>("holiday"));

        // MediatR registrations
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(Program).Assembly,
                typeof(GetHolidayPackagesQueryHandler).Assembly,
                typeof(GetTourPackagesQueryHandler).Assembly,
                typeof(GetProductsQueryHandler).Assembly
            ));

        return services;
    }

    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}