using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Common.Infrastructure;

public static class MasstransitExtensions
{
    public static WebApplicationBuilder AddCustomMasstransit(this WebApplicationBuilder builder,
        Action<IBusRegistrationConfigurator>? configure = null)
    {
        var rabbitMqHostName = builder.Configuration.GetValue<string>("RABBITMQ_HOSTNAME");

        builder.Services.AddMassTransit(x =>
        {
            configure?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHostName,"/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        return builder;
    }
}