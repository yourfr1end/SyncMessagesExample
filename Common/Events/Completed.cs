namespace Common.Events;

public interface ICompleted
{
    Guid InstanceId { get; }
}

public record Completed(Guid InstanceId) : ICompleted;