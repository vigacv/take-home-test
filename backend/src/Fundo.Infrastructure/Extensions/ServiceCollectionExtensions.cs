using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFundoSqlServer(
        this IServiceCollection services, string connectionString) =>
        services.AddDbContext<FundoDbContext>(options =>
            options.UseSqlServer(connectionString));

    public static IServiceCollection AddFundoInMemory(
        this IServiceCollection services, string databaseName) =>
        services.AddDbContext<FundoDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));

    public static void InitializeFundoDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FundoDbContext>();
        if (db.Database.IsRelational())
        {
            db.Database.Migrate();
        }
        else
        {
            db.Database.EnsureCreated();
            if (!db.Loans.Any())
            {
                db.Loans.AddRange(FundoDbContext.SeedLoans);
                db.SaveChanges();
            }
        }
    }
}
