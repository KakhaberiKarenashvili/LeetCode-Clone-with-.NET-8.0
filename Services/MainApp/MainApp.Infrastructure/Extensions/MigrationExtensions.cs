using MainApp.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MainApp.Infrastructure.Extensions;

public static class MigrationExtensions
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
        
        try
        {
            using AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            logger.LogInformation("Starting database migration...");
            
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Ensure database is created
             context.Database.EnsureCreatedAsync();
            
            // Check if there are pending migrations
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Found {Count} pending migrations. Applying...", pendingMigrations.Count());
                dbContext.Database.Migrate();
                logger.LogInformation("Database migration completed successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found. Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }


    }
    
}