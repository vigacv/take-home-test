using Fundo.Applications.WebApi.Middleware;
using Fundo.Applications.WebApi.Services;
using Fundo.Domain.Repositories;
using Fundo.Infrastructure.Extensions;
using Fundo.Infrastructure.Repositories;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

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

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseCors();
    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}