using System.Text.Json;
using Common.Events;
using MassTransit;

namespace SyncMessagesExample.Api.StateMachine;

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

        Initially(WhenSyncSubmittedReceived);

        During(GettingInstanceInfo, WhenGotInstanceInfoReceived, WhenSyncFailedReceived);
        During(GettingMessagesToSync, WhenMessagesToSyncReceived, WhenSyncFailedReceived);
        During(GettingMessagesStatus, WhenGotMessageStatusesReceived, WhenSyncFailedReceived);
        During(UpdatingMessagesStatus, WhenMessageUpdatedReceived, WhenSyncFailedReceived);
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

    private EventActivityBinder<SyncMessagesState, ISubmitSync> WhenSyncSubmittedReceived =>
        When(SyncInstanceSubmitted)
            .Then(context =>
            {
                _logger.LogInformation("Initializing statemachine for instance = {InstanceId}",
                    context.Message.InstanceId);

                context.Saga.InstanceId = context.Message.InstanceId;
                context.Saga.StartSyncing = DateTime.UtcNow;
            })
            .PublishAsync(context =>
                context.Init<IGetInstanceInfo>(new GetInstanceInfo(context.Message.InstanceId)))
            .TransitionTo(GettingInstanceInfo);

    private EventActivityBinder<SyncMessagesState, IGotInstanceInfo> WhenGotInstanceInfoReceived =>
        When(GotInstanceInfo)
            .PublishAsync(context =>
                context.Init<IGetMessagesToSync>(new GetMessagesToSync(context.Message.InstanceId)))
            .TransitionTo(GettingMessagesToSync);

    private EventActivityBinder<SyncMessagesState, IGotMessagesToSync> WhenMessagesToSyncReceived =>
        When(GotMessagesToSync)
            .PublishAsync(context =>
                context.Init<IGetMessageStatuses>(new GetMessageStatuses(context.Message.InstanceId,
                    context.Message.MessageIdsToSync)))
            .TransitionTo(GettingMessagesStatus);

    private EventActivityBinder<SyncMessagesState, IGotMessageStatuses> WhenGotMessageStatusesReceived =>
        When(GotMessagesStatuses)
            .Then(context =>
            {
                context.Saga.MessagesStates =
                    JsonSerializer.Serialize(context.Message.Statuses);
            })
            .PublishAsync(context =>
                context.Init<IUpdateMessages>(new UpdateMessages(context.Message.InstanceId,
                    context.Message.Statuses)))
            .TransitionTo(UpdatingMessagesStatus);

    private EventActivityBinder<SyncMessagesState, IMessageUpdated> WhenMessageUpdatedReceived => When(MessagesUpdated)
        .Then(context =>
        {
            _logger.LogInformation("Messages statuses updated for Instance = {InstanceId}",
                context.Message.InstanceId);

            context.Saga.EndSyncing = DateTime.UtcNow;
        })
        .TransitionTo(Completed)
        .Finalize();

    private EventActivityBinder<SyncMessagesState, IFailed> WhenSyncFailedReceived => When(SyncFailed)
        .Then(context =>
        {
            _logger.LogError("Sync failed for Instance = {InstanceId}. With message = {Message}",
                context.Message.InstanceId, context.Message.Message);
            context.Saga.EndSyncing = DateTime.UtcNow;
            context.Saga.FailedReason = context.Message.Message;
        })
        .TransitionTo(Failed)
        .Finalize();
}