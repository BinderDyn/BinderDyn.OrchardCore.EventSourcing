using BinderDyn.OrchardCore.EventSourcing.Enums;

namespace BinderDyn.OrchardCore.EventSourcing.Models;

public class EventFilter
{
    public EventState[] States { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 30;
}