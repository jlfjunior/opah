using Microsoft.EntityFrameworkCore;
using Opah.Redis.Client;
using Opah.Transaction.API;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TransactionDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TransactionDb")));

// Redis Dependency Injection
builder.Services.Configure<RedisClientOptions>(builder.Configuration.GetSection(RedisClientOptions.Section));
builder.Services.AddScoped<IStreamPublisher, StreamPublisher>();

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

app.Run();

