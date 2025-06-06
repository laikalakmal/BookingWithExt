using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Application.Interfaces;
using Core.Application.DTOs;
using Core.Application.Services;
using Core.Application.Features.Products.Queries.GetHolidayPackages;
using Core.Application.Features.Products.Queries.GetTourPackages;
using Core.Application.Features.Products.Queries.GetAllProducts;

namespace ProductServiceAPI.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ProductDb")));

            // Generic product repository
            services.AddScoped<IProductRepository<Product>, GenericProductRepository>();

            // Register all product types
            services.RegisterTourPackageInfrastructure()
                    .RegisterHolidayPackageInfrastructure()
                    .RegisterCustomProductInfrastructure();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Generic product service
            services.AddScoped<IProductService<Product, ProductDto>, GenericProductService>();

            // Register all product types
            services.RegisterTourPackageServices()
                    .RegisterHolidayPackageServices()
                    .RegisterCustomProductServices();

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
}