using Common.Events;
using MassTransit;

namespace MessagesService.Consumers;

public class UpdateMessageConsumer : IConsumer<IUpdateMessages>
{
    private readonly ILogger<UpdateMessageConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateMessageConsumer(ILogger<UpdateMessageConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IUpdateMessages> context)
    {
        _logger.LogInformation("Updating messages statuses for instance = {InstanceId}", context.Message.InstanceId);

        await Task.Delay(1000);

        if (Random.Shared.Next(100) < 95)
        {
            await _publishEndpoint.Publish<IMessageUpdated>(new MessageUpdated(context.Message.InstanceId));
        }
        else
        {
            await _publishEndpoint.Publish<IFailed>(new Failed(context.Message.InstanceId,
                "Failed to update messages"));
        }
    }
}