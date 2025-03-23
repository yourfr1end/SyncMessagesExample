using MassTransit;

namespace SyncMessagesExample.Api.StateMachine;

public class SyncMessagesState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    // TODO: implement model
}