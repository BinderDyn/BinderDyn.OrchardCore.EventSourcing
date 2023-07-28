using BinderDyn.OrchardCore.EventSourcing.Enums;

namespace BinderDyn.OrchardCore.EventSourcing.ViewModels;

public class EventTableViewModel
{
    public EventViewModel[] Events { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int MaxPage { get; set; }
    public string BaseUrl { get; set; }
    public EventState State { get; set; }
}