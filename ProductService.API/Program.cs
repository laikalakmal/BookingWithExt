using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ProductServiceAPI.Infrastructure.DependencyInjection;

namespace ProductServiceAPI
{
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

            // Add health checks
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>();

            var app = builder.Build();

            // Apply migrations if specified or in development mode
            if (args.Length > 0 && args[0] == "--apply-migrations" || app.Environment.IsDevelopment())
            {
                try
                {
                    using var scope = app.Services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    // Check if there are any pending migrations before applying
                    var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation("Applying {Count} pending migrations: {Migrations}",
                            pendingMigrations.Count,
                            string.Join(", ", pendingMigrations));
                            
                        // Only delete database in development to preserve production data
                        if (app.Environment.IsDevelopment() && args.Length > 0 && args[0] == "--apply-migrations")
                        {
                            dbContext.Database.EnsureDeleted();
                            logger.LogWarning("Database was deleted before applying migrations - DEV ENVIRONMENT ONLY");
                        }
                        
                        dbContext.Database.Migrate();
                    }
                    else
                    {
                        logger.LogInformation("No pending migrations to apply");
                    }
                }
                catch (Exception ex)
                {
                    var logger = app.Services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database");

                    // In development, we might want to see the error, but in production we should continue
                    if (!app.Environment.IsDevelopment())
                    {
                        // Continue application startup even if migrations failed
                        logger.LogWarning("Application continuing despite migration failure");
                    }
                    else
                    {
                        throw; // Rethrow in development to see the full error
                    }
                }
            }

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Map health check endpoint
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}