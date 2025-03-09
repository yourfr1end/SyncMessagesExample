using Common.Events;
using MassTransit;

namespace GetInstanceInfoService.Consumers;

public class GetInstanceInfoConsumer : IConsumer<IGetInstanceInfo>
{
    private readonly ILogger<GetInstanceInfoConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public GetInstanceInfoConsumer(ILogger<GetInstanceInfoConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IGetInstanceInfo> context)
    {
        _logger.LogInformation("Getting info for instance: {InstanceId}", context.Message.InstanceId);

        await Task.Delay(1000);

        if (Random.Shared.Next(100) < 95)
        {
            var instanceInfo = new GotInstanceInfo(context.Message.InstanceId, $"InstanceName_{Guid.NewGuid()}",
                $"email_{Random.Shared.Next(100)}@test.test");

            await _publishEndpoint.Publish<IGotInstanceInfo>(instanceInfo);
        }
        else
        {
            await _publishEndpoint.Publish<IFailed>(new Failed(context.Message.InstanceId,
                "Failed to get instance info"));
        }
    }
}