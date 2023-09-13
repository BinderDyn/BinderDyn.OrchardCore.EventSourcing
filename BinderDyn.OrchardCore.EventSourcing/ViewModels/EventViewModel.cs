using BinderDyn.OrchardCore.EventSourcing.Abstractions.Enums;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;

namespace BinderDyn.OrchardCore.EventSourcing.ViewModels;

public class EventViewModel
{
    public EventViewModel()
    {
    }

    public EventViewModel(Event eventData)
    {
        var createdSpecifiedAsUtc = new DateTime(eventData.CreatedUtc.Year, eventData.CreatedUtc.Month,
            eventData.CreatedUtc.Day, eventData.CreatedUtc.Hour, eventData.CreatedUtc.Minute,
            eventData.CreatedUtc.Second, DateTimeKind.Utc);
        var processedSpecifiedAsUtc =
            eventData.ProcessedUtc.HasValue
                ? new DateTime(eventData.ProcessedUtc.Value.Year, eventData.ProcessedUtc.Value.Month,
                    eventData.ProcessedUtc.Value.Day,
                    eventData.ProcessedUtc.Value.Hour, eventData.ProcessedUtc.Value.Minute,
                    eventData.ProcessedUtc.Value.Second,
                    DateTimeKind.Utc)
                : (DateTime?)null;

        EventId = eventData.EventId;
        FriendlyName = eventData.EventTypeFriendlyName;
        Processed = processedSpecifiedAsUtc;
        Created = createdSpecifiedAsUtc;
        PayloadType = eventData.PayloadType;
        ReferenceId = eventData.ReferenceId;
        EventState = eventData.EventState;
        Payload = eventData.Payload;
    }

    public Guid EventId { get; set; } = Guid.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public string PayloadType { get; set; } = string.Empty;
    public string Payload { get; set; }
    public DateTime Created { get; set; } = DateTime.MinValue;
    public DateTime? Processed { get; set; }
    public EventState EventState { get; set; } = EventState.Pending;
}