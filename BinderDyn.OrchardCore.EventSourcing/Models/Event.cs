using BinderDyn.OrchardCore.EventSourcing.Enums;

namespace BinderDyn.OrchardCore.EventSourcing.Models;

public interface IEvent
{
    public Guid EventId { get; set; }
    public Guid? OriginalEventId { get; set; }
    public string? ReferenceId { get; set; }
    public string PayloadType { get; set; }
    public string EventTypeFriendlyName { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public EventState EventState { get; set; }
}

public class Event : IEvent
{
    public Guid EventId { get; set; }
    public Guid? OriginalEventId { get; set; }
    public string? ReferenceId { get; set; }
    public string Payload { get; set; }
    public string PayloadType { get; set; }
    public string EventTypeFriendlyName { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public EventState EventState { get; set; }
}