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
    public string? FailedReason { get; set; }
}

public class SyncMessagesStateMachine : MassTransitStateMachine<SyncMessagesState>
{
    public SyncMessagesStateMachine()
    {
        InstanceState(x => x.CurrentState);
        
        Initially(When(SyncInstanceSubmitted)
            .Then(context =>
            {
                
            })
            .PublishAsync(context => context.Init<IGetInstanceInfo>(new GetInstanceInfo(default)))
            .TransitionTo(GettingInstanceInfo)
        );
    }

    public State GettingInstanceInfo { get; private set; } = null!;
    public State GettingMessagesToSync { get; private set; } = null!;
    public State GettingMessagesStatus { get; private set; } = null!;
    public State UpdatingMessagesStatus { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;
    
    public Event<> SyncInstanceSubmitted { get; private set; } = null!;
    public Event<> GotInstanceInfo { get; private set; } = null!;
    public Event<> GotMessagesToSync { get; private set; } = null!;
    public Event<> GotMessagesStatuses { get; private set; } = null!;
    public Event<> MessagesUpdated { get; private set; } = null!;
    public Event<> SyncFailed { get; private set; } = null!;
}