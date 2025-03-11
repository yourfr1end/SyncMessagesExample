using Common.Events;
using Common.Infrastructure;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Saga;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SyncMessagesExample.Api.StateMachine;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddCustomOpenTelemetry("SyncMessagesExample.Api");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddCustomMasstransit(x =>
    x.AddSagaStateMachine<SyncMessagesStateMachine, SyncMessagesState>()
        .InMemoryRepository()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/start-syncing", (IPublishEndpoint publishEndpoint) =>
    {
        var guid = Guid.NewGuid();
        publishEndpoint.Publish<ISubmitSync>(new SubmitSync(guid));

        return Results.Ok(guid);
    })
    .WithName("StartSyncing")
    .WithOpenApi();

app.MapGet("/get-saga/{correlationId}", async (ILoadSagaRepository<SyncMessagesState> repo,
        [FromQuery] Guid correlationId) =>
    {
        var state = await repo.Load(correlationId);
        return state;
    })
    .WithName("GetSagas")
    .WithOpenApi();

app.Run();