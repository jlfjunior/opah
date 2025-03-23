using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.Infrastructure;
using Opah.Consolidation.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<TransactionService>();

builder.Services.AddDbContext<ConsolidationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConsolidationDb")));

builder.Services.AddHostedService<TransactionCreatedWorker>();

var host = builder.Build();
host.Run();