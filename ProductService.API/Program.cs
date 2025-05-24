using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
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
