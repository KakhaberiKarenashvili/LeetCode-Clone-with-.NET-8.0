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

        logger.LogInformation("Starting database migration...");

        AppDbContext? dbContext = null;
        try
        {
            dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count != 0)
            {
                logger.LogInformation("Found {Count} pending migrations. Applying...", pendingMigrations.Count);
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
        finally
        {
            try { dbContext?.Dispose(); }
            catch { /* suppress dispose errors when connection never opened */ }
        }
    }
    
}