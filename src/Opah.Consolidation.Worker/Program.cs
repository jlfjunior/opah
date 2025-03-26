using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.Application;
using Opah.Consolidation.Infrastructure;
using Opah.Consolidation.Worker;
using Opah.Redis.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IDailyClosureService, IDailyClosureService>();

builder.Services.AddAutoMapper(typeof(TransactionProfile), typeof(DailyClosureProfile));

builder.Services.AddDbContext<ConsolidationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConsolidationDb")));

builder.Services.AddHostedService<TransactionCreatedWorker>();
// Redis Dependency Injection
builder.Services.Configure<RedisClientOptions>(builder.Configuration.GetSection(RedisClientOptions.Section));
builder.Services.AddScoped<IStreamPublisher, StreamPublisher>();

var host = builder.Build();
host.Run();