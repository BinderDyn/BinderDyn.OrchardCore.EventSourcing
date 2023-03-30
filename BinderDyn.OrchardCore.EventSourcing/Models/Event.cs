using BinderDyn.OrchardCore.EventSourcing.Enums;

namespace BinderDyn.OrchardCore.EventSourcing.Models;

public class Event<T>
{
    public Guid EventId { get; set; }
    public Guid? OriginalEventId { get; set; }
    public string? ReferenceId { get; set; }
    public T Payload { get; set; }
    public string PayloadType { get; set; }
    public string EventTypeFriendlyName { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Processed { get; set; }
    public EventState EventState { get; set; }
}