using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        const int maxRetries = 10;
        int retryCount = 0;

        while (true)
        {
            try
            {
                await db.Database.MigrateAsync();
                await DbInitializer.SeedAsync(db);
                break;
            }
            catch (Exception ex)
            {
                retryCount++;

                if (retryCount >= maxRetries)
                    throw;

                Console.WriteLine($"DB connection failed. Retrying {retryCount}/{maxRetries}...");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }

}
