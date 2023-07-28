using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using Microsoft.EntityFrameworkCore;

namespace BinderDyn.OrchardCore.EventSourcing.Models;

public interface IEvent
{
    public Guid EventId { get; set; }
    public Guid? OriginalEventId { get; }
    public string? ReferenceId { get; set; }
    public string PayloadType { get; set; }
    public string EventTypeFriendlyName { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public EventState EventState { get; set; }
}

[Table("Events")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Event : IEvent
{
    [Key] public Guid EventId { get; set; }
    public virtual Event? OriginalEvent { get; set; }

    [NotMapped] public Guid? OriginalEventId => OriginalEvent?.OriginalEventId;
    public string? ReferenceId { get; set; }
    public string Payload { get; set; } = string.Empty;
    public string PayloadType { get; set; } = string.Empty;
    public string EventTypeFriendlyName { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public EventState EventState { get; set; }

    public void Update(Event newEventData)
    {
        ReferenceId = newEventData.ReferenceId;
        PayloadType = newEventData.PayloadType;
        Payload = newEventData.Payload;
        EventTypeFriendlyName = newEventData.EventTypeFriendlyName;
        CreatedUtc = newEventData.CreatedUtc;
        ProcessedUtc = DateTime.UtcNow;
        EventState = newEventData.EventState;
    }

    public class EventCreationParam
    {
        public virtual Guid? OriginalEventId { get; set; }
        public string? ReferenceId { get; set; }
        public string Payload { get; set; } = string.Empty;
        public string PayloadType { get; set; } = string.Empty;
        public string EventTypeFriendlyName { get; set; } = string.Empty;
    }
}