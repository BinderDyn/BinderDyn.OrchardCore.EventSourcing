namespace BinderDyn.OrchardCore.EventSourcing.Models;

public class Event<T>
{
    public Guid EventId { get; set; }
    public Guid? OriginalEventId { get; set; }
    public T Payload { get; set; }
    public string PayloadType { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Processed { get; set; }
}