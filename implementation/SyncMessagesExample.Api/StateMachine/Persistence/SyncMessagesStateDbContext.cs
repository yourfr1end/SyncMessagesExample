using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace SyncMessagesExample.Api.StateMachine.Persistence;

public class SyncMessagesStateDbContext : SagaDbContext
{
    public SyncMessagesStateDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new SyncMessagesStateMap(); }
    }
}