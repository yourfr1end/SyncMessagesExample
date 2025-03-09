namespace Common.Events;

public interface IUpdateMessages
{
    Guid InstanceId { get; }
    MessageStatus[] MessageStatuses { get; }
}

public interface IMessageUpdated
{
    Guid InstanceId { get; }
}

public record UpdateMessages(Guid InstanceId, MessageStatus[] MessageStatuses) : IUpdateMessages;

public record MessageUpdated(Guid InstanceId) : IMessageUpdated;