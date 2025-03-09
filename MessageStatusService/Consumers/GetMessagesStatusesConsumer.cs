using Common.Events;
using MassTransit;

namespace MessageStatusService.Consumers;

public class GetMessagesStatusesConsumer : IConsumer<IGetMessageStatuses>
{
    private readonly ILogger<GetMessagesStatusesConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public GetMessagesStatusesConsumer(ILogger<GetMessagesStatusesConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IGetMessageStatuses> context)
    {
        _logger.LogInformation("Getting messages statuses for instance = {InstanceId}", context.Message.InstanceId);

        await Task.Delay(1000);

        if (Random.Shared.Next(100) < 95)
        {
            var statuses = context.Message.MessageIds
                .Select(id => Random.Shared.Next(100) < 50
                    ? new MessageStatus(id, MessageStatusEnum.Answered)
                    : new MessageStatus(id, MessageStatusEnum.Unanswered))
                .ToArray();

            await _publishEndpoint.Publish<IGotMessageStatuses>(new GotMessageStatuses(context.Message.InstanceId,
                statuses));
        }
        else
        {
            await _publishEndpoint.Publish<IFailed>(new Failed(context.Message.InstanceId,
                "Failed to get messages statuses"));
        }
    }
}