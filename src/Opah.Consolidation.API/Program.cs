using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.API;
using Opah.Consolidation.Application;
using Opah.Consolidation.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ConsolidationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConsolidationDb")));
builder.Services.AddAutoMapper(typeof(TransactionProfile), typeof(DailyClosureProfile));
builder.Services.AddScoped<IDailyClosureService, DailyClosureService>();

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ConsolidationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

public partial class Program { }