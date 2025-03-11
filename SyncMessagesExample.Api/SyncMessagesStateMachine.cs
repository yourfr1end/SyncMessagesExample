using Common.Events;
using MassTransit;

namespace SyncMessagesExample.Api;

public class SyncMessagesState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public Guid InstanceId { get; set; }
    public DateTime StartSyncing { get; set; }
    public DateTime? EndSyncing { get; set; }
    public string MessagesStates { get; set; }
    public string? FailedReason { get; set; }
}

public class SyncMessagesStateMachine : MassTransitStateMachine<SyncMessagesState>
{
    private readonly ILogger<SyncMessagesStateMachine> _logger;

    public SyncMessagesStateMachine(ILogger<SyncMessagesStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState);

        Event(() => SyncInstanceSubmitted, x => x.CorrelateById(m => m.Message.InstanceId));
        Event(() => GotInstanceInfo, x => x.CorrelateById(m => m.Message.InstanceId));
        Event(() => GotMessagesToSync, x => x.CorrelateById(m => m.Message.InstanceId));
        Event(() => GotMessagesStatuses, x => x.CorrelateById(m => m.Message.InstanceId));
        Event(() => MessagesUpdated, x => x.CorrelateById(m => m.Message.InstanceId));
        Event(() => SyncFailed, x => x.CorrelateById(m => m.Message.InstanceId));

        Initially(When(SyncInstanceSubmitted)
            .Then(context =>
            {
                _logger.LogInformation("Initializing statemachine for instance = {InstanceId}",
                    context.Message.InstanceId);

                context.Saga.InstanceId = context.Message.InstanceId;
                context.Saga.StartSyncing = DateTime.UtcNow;
            })
            .PublishAsync(context =>
                context.Init<IGetInstanceInfo>(new GetInstanceInfo(context.Message.InstanceId)))
            .TransitionTo(GettingInstanceInfo)
        );

        During(GettingInstanceInfo,
            When(GotInstanceInfo)
                .PublishAsync(context =>
                    context.Init<IGetMessagesToSync>(new GetMessagesToSync(context.Message.InstanceId)))
                .TransitionTo(GettingMessagesToSync),
            When(SyncFailed)
                .Then(context =>
                {
                    _logger.LogError("Sync failed for Instance = {InstanceId}. With message = {Message}",
                        context.Message.InstanceId, context.Message.Message);
                    context.Saga.EndSyncing = DateTime.UtcNow;
                    context.Saga.FailedReason = context.Message.Message;
                })
                .TransitionTo(Failed)
                .Finalize()
        );

        During(GettingMessagesToSync,
            When(GotMessagesToSync)
                .PublishAsync(context =>
                    context.Init<IGetMessageStatuses>(new GetMessageStatuses(context.Message.InstanceId,
                        context.Message.MessageIdsToSync)))
                .TransitionTo(GettingMessagesStatus),
            When(SyncFailed)
                .Then(context =>
                {
                    _logger.LogError("Sync failed for Instance = {InstanceId}. With message = {Message}",
                        context.Message.InstanceId, context.Message.Message);
                    context.Saga.EndSyncing = DateTime.UtcNow;
                    context.Saga.FailedReason = context.Message.Message;
                })
                .TransitionTo(Failed)
                .Finalize()
        );

        During(GettingMessagesStatus,
            When(GotMessagesStatuses)
                .Then(context =>
                {
                    context.Saga.MessagesStates =
                        System.Text.Json.JsonSerializer.Serialize(context.Message.Statuses);
                })
                .PublishAsync(context =>
                    context.Init<IUpdateMessages>(new UpdateMessages(context.Message.InstanceId,
                        context.Message.Statuses)))
                .TransitionTo(UpdatingMessagesStatus),
            When(SyncFailed)
                .Then(context =>
                {
                    _logger.LogError("Sync failed for Instance = {InstanceId}. With message = {Message}",
                        context.Message.InstanceId, context.Message.Message);
                    context.Saga.EndSyncing = DateTime.UtcNow;
                    context.Saga.FailedReason = context.Message.Message;
                })
                .TransitionTo(Failed)
                .Finalize()
        );

        During(UpdatingMessagesStatus,
            When(MessagesUpdated)
                .Then(context =>
                {
                    _logger.LogInformation("Messages statuses updated for Instance = {InstanceId}",
                        context.Message.InstanceId);

                    context.Saga.EndSyncing = DateTime.UtcNow;
                })
                .TransitionTo(Completed)
                .Finalize(),
            When(SyncFailed)
                .Then(context =>
                {
                    _logger.LogError("Sync failed for Instance = {InstanceId}. With message = {Message}",
                        context.Message.InstanceId, context.Message.Message);
                    context.Saga.EndSyncing = DateTime.UtcNow;
                    context.Saga.FailedReason = context.Message.Message;
                })
                .TransitionTo(Failed)
                .Finalize()
        );
    }

    public State GettingInstanceInfo { get; private set; } = null!;
    public State GettingMessagesToSync { get; private set; } = null!;
    public State GettingMessagesStatus { get; private set; } = null!;
    public State UpdatingMessagesStatus { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public Event<ISubmitSync> SyncInstanceSubmitted { get; private set; } = null!;
    public Event<IGotInstanceInfo> GotInstanceInfo { get; private set; } = null!;
    public Event<IGotMessagesToSync> GotMessagesToSync { get; private set; } = null!;
    public Event<IGotMessageStatuses> GotMessagesStatuses { get; private set; } = null!;
    public Event<IMessageUpdated> MessagesUpdated { get; private set; } = null!;
    public Event<IFailed> SyncFailed { get; private set; } = null!;
}