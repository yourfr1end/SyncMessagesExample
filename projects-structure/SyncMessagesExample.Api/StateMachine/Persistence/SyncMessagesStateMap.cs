using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SyncMessagesExample.Api.StateMachine.Persistence;

public class SyncMessagesStateMap : SagaClassMap<SyncMessagesState>
{
    protected override void Configure(EntityTypeBuilder<SyncMessagesState> entity, ModelBuilder model)
    {
        // TODO: implement model class map
    }
}