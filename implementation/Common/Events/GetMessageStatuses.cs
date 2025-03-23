namespace Common.Events;

public interface IGetMessageStatuses
{
    Guid InstanceId { get; }
    string[] MessageIds { get; }
}

public interface IGotMessageStatuses
{
    Guid InstanceId { get; }
    MessageStatus[] Statuses { get; }
}

public record GetMessageStatuses(Guid InstanceId, string[] MessageIds) : IGetMessageStatuses;

public record GotMessageStatuses(Guid InstanceId, MessageStatus[] Statuses) : IGotMessageStatuses;

public record MessageStatus(string MessageId, MessageStatusEnum Status);

public enum MessageStatusEnum
{
    Answered,
    Unanswered,
}