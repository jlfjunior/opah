using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.API;
using Opah.Consolidation.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ConsolidationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConsolidationDb")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

app.MapDailyClosureEndpoints();

app.Run();