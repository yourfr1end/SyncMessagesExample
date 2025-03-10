using Common.Events;
using MassTransit;
using MassTransit.Saga;
using Microsoft.AspNetCore.Mvc;
using SyncMessagesExample.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<SyncMessagesStateMachine, SyncMessagesState>()
        .InMemoryRepository();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

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
        var guid =  Guid.NewGuid();
        publishEndpoint.Publish<ISubmitSync>(new SubmitSync(guid));

        return Results.Ok(guid);
    })
    .WithName("StartSyncing")
    .WithOpenApi();

app.MapGet("/get-saga/{correlationId}", async (ILoadSagaRepository<SyncMessagesState> repo,
    [FromQuery]Guid correlationId) =>
    {
        var state = await repo.Load(correlationId);
        return state;
    })
    .WithName("GetSagas")
    .WithOpenApi();

app.Run();