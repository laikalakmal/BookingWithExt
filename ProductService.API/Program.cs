using Core.Application.DTOs;
using Core.Application.Features.Products.Queries.GetAllProducts;
using Core.Application.Features.Products.Queries.GetHolidayPackages;
using Core.Application.Features.Products.Queries.GetTourPackages;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Domain.Entities;
using Infrastructure.Adapters;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register services by architectural layer
        builder.Services
            .AddInfrastructureServices(builder.Configuration)
            .AddApplicationServices()
            .AddWebApiServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

// Extension methods for clean service registration by layer
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database context
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ProductDb")));

        // Repositories
        services.AddKeyedScoped<IProductRepository<TourPackage>, TourPackageRepository>("tour");
        services.AddKeyedScoped<IProductRepository<HolidayPackage>, HolidayPackageRepository>("holiday");

        // External API adapters
        services.AddKeyedScoped<IExternalProductApiAdapter, TourApiAdapter>("tour");
        services.AddKeyedScoped<IExternalProductApiAdapter, HolidayPackageAdapter>("holiday");

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register product services with keyed DI
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