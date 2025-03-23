using Common.Infrastructure;
using GetInstanceInfoService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddCustomOpenTelemetry("GetInstanceInfoService");

builder.AddCustomMasstransit(x => x.AddConsumer<GetInstanceInfoConsumer>());

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();