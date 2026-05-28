using Fundo.Applications.WebApi.Services;
using Fundo.Domain.Repositories;
using Fundo.Infrastructure.Extensions;
using Fundo.Infrastructure.Repositories;

namespace Fundo.Applications.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                    opts.JsonSerializerOptions.PropertyNamingPolicy =
                        System.Text.Json.JsonNamingPolicy.CamelCase);

            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            if (builder.Environment.IsEnvironment("Testing"))
            {
                var dbName = builder.Configuration["Testing:DbName"] ?? "FundoTest";
                builder.Services.AddFundoInMemory(dbName);
            }
            else
            {
                builder.Services.AddFundoSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")!);
            }

            builder.Services.AddScoped<ILoanRepository, LoanRepository>();
            builder.Services.AddScoped<ILoanService, LoanService>();

            var app = builder.Build();

            app.Services.InitializeFundoDatabase();

            app.UseCors();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
