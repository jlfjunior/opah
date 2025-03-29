using Microsoft.EntityFrameworkCore;
using Opah.Redis.Client;
using Opah.Transaction.API;
using Opah.Transaction.API.Business;
using Opah.Transaction.API.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TransactionDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TransactionDb")));

// Redis Dependency Injection
builder.Services.Configure<RedisClientOptions>(builder.Configuration.GetSection(RedisClientOptions.Section));
builder.Services.AddScoped<IStreamPublisher, StreamPublisher>();
builder.Services.AddAutoMapper(typeof(TransactionProfile));
// builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// app.UseHttpsRedirection();

app.MapTransactionEndpoints();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    var pendingMigrations = dbContext.Database.GetPendingMigrations();
    if (pendingMigrations.Any())
    {
        dbContext.Database.Migrate();
    }
}
app.Run();

public partial class Program { }