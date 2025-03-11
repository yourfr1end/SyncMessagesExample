using Common.Infrastructure;
using GetInstanceInfoService.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.AddCustomOpenTelemetry("GetInstanceInfoService");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddCustomMasstransit(x => x.AddConsumer<GetInstanceInfoConsumer>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();