using Common.Infrastructure;
using MessagesService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddCustomOpenTelemetry("MessagesService");

builder.AddCustomMasstransit(x =>
{
    x.AddConsumer<GetMessagesToSyncConsumer>();
    x.AddConsumer<UpdateMessageConsumer>();
});

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();