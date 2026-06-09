using FCG.Catalog.Application;
using FCG.Catalog.Infrastructure;
using FCG.Catalog.Infrastructure.Data;
using FCG.Catalog.Worker;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecksInfrastructure(builder.Configuration);

builder.Services.Configure<PaymentProcessedWorkerConfig>(
    builder.Configuration.GetSection("Workers:PaymentProcessed"));
builder.Services.AddHostedService<PaymentProcessedWorker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapHealthChecks("/health");
app.Run();
