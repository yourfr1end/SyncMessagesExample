using Common.Infrastructure;
using GetInstanceInfoService.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddCustomOpenTelemetry("GetInstanceInfoService");

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();