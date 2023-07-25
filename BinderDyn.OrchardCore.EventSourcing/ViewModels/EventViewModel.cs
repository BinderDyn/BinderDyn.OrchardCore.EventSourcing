using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;

namespace BinderDyn.OrchardCore.EventSourcing.ViewModels;

public class EventViewModel
{
    public EventViewModel()
    {
    }

    public EventViewModel(Event eventData)
    {
        EventId = eventData.EventId;
        FriendlyName = eventData.EventTypeFriendlyName;
        Processed = eventData.ProcessedUtc;
        Created = eventData.CreatedUtc;
        PayloadType = eventData.PayloadType;
        ReferenceId = eventData.ReferenceId;
        EventState = eventData.EventState;
    }

    public Guid EventId { get; set; } = Guid.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public string PayloadType { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.MinValue;
    public DateTime? Processed { get; set; }
    public EventState EventState { get; set; } = EventState.Pending;
}