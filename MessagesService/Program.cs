using Common.Infrastructure;
using MassTransit;
using MessagesService.Consumers;

var builder = WebApplication.CreateBuilder(args);
builder.AddCustomOpenTelemetry("MessagesService");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddCustomMasstransit(x =>
{
    x.AddConsumer<GetMessagesToSyncConsumer>();
    x.AddConsumer<UpdateMessageConsumer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();