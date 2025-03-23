namespace Common.Events;

public interface ISubmitSync
{
    Guid InstanceId { get; }
}

public record SubmitSync(Guid InstanceId) : ISubmitSync;