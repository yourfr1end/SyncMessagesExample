namespace Common.Events;

public interface IFailed
{
    Guid InstanceId { get; }
    string Message { get; }
}

public record Failed(Guid InstanceId, string Message) : IFailed;