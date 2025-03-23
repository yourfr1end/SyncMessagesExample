using Common.Infrastructure;
using MassTransit;
using MessageStatusService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddCustomOpenTelemetry("MessageStatusService");

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();