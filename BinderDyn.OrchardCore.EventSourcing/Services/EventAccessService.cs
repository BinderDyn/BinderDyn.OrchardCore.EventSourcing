using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.ViewModels;

namespace BinderDyn.OrchardCore.EventSourcing.Services;

public interface IEventAccessService
{
    Task<EventViewModel[]> GetAllFiltered(EventFilter filter);
    Task RescheduleEventForPending(Guid id);
}

public class EventAccessService : IEventAccessService
{
    private readonly IEventRepository _eventRepository;

    public EventAccessService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventViewModel[]> GetAllFiltered(EventFilter filter)
    {
        throw new NotImplementedException();
    }

    public async Task RescheduleEventForPending(Guid id)
    {
        throw new NotImplementedException();
    }
}