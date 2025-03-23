using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SyncMessagesExample.Api.StateMachine.Persistence;

public class SyncMessagesStateMap : SagaClassMap<SyncMessagesState>
{
    protected override void Configure(EntityTypeBuilder<SyncMessagesState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.MessagesStates);
        entity.Property(x => x.StartSyncing);
        entity.Property(x => x.EndSyncing);
        entity.Property(x => x.InstanceId);
        entity.Property(x => x.FailedReason);
    }
}