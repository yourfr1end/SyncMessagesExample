using Common.Events;
using MassTransit;

namespace MessagesService.Consumers;

public class GetMessagesToSyncConsumer : IConsumer<IGetMessagesToSync>
{
    private readonly ILogger<GetMessagesToSyncConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public GetMessagesToSyncConsumer(ILogger<GetMessagesToSyncConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IGetMessagesToSync> context)
    {
        _logger.LogInformation("Getting messages to sync for instance={InstanceId}", context.Message.InstanceId);

        await Task.Delay(1000);

        if (Random.Shared.Next(100) < 95)
        {
            var messageIds = Enumerable.Range(0, 10).Select(x => $"{Guid.NewGuid()}").ToArray();

            await _publishEndpoint.Publish<IGotMessagesToSync>(new GotMessagesToSync(context.Message.InstanceId,
                messageIds));
        }
        else
        {
            await _publishEndpoint.Publish<IFailed>(new Failed(context.Message.InstanceId,
                "Failed to get messages to sync"));
        }
    }
}