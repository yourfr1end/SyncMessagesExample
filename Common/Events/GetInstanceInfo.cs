namespace Common.Events;

public interface IGetInstanceInfo
{
    Guid InstanceId { get; }
}

public interface IGotInstanceInfo
{
    Guid InstanceId { get; }
    string InstanceName { get; }
    string Email { get; }
}

public record GetInstanceInfo(Guid InstanceId) : IGetInstanceInfo;

public record GotInstanceInfo(
    Guid InstanceId,
    string InstanceName,
    string Email) : IGotInstanceInfo;