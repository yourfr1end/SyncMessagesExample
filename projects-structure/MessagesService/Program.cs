using Common.Infrastructure;
using MassTransit;
using MessagesService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddCustomOpenTelemetry("MessagesService");

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();