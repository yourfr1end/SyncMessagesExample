using MassTransit;
using Microsoft.AspNetCore.Builder;

namespace Common.Infrastructure;

public static class MasstransitExtensions
{
    public static WebApplicationBuilder AddCustomMasstransit(this WebApplicationBuilder builder,
        Action<IBusRegistrationConfigurator>? configure = null)
    {
        builder.Services.AddMassTransit(x =>
        {
            configure?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("messagebroker-sevice","/", h =>
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