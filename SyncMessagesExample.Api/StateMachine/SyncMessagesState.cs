using MassTransit;

namespace SyncMessagesExample.Api.StateMachine;

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