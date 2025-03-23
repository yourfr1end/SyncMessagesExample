using Common.Infrastructure;
using MessageStatusService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddCustomOpenTelemetry("MessageStatusService");

builder.AddCustomMasstransit(x => x.AddConsumer<GetMessagesStatusesConsumer>());

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();