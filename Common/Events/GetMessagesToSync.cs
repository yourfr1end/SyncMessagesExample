namespace Common.Events;

public interface IGetMessagesToSync
{
    Guid InstanceId { get; }
}

public interface IGotMessagesToSync
{
    Guid InstanceId { get; }
    string[] MessageIdsToSync { get; }
}

public record GetMessagesToSync(Guid InstanceId) : IGetMessagesToSync;

public record GotMessagesToSync(Guid InstanceId, string[] MessageIdsToSync) : IGotMessagesToSync;