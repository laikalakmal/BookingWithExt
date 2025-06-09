using Core.Application.Features.Products.Queries.GetAllProducts;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ProductServiceAPI.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ProductDb")));

           

            // Register all product types
            services.RegisterProductInfrastructure();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {


            // Register all product types

            services.RegisterProductServices();
            // MediatR registrations
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(
                    typeof(Program).Assembly,
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