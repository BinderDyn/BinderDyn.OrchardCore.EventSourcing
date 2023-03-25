using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventRepository
{
    Task Add<T>(Event<T> eventData);
    Task Add<T>(IEnumerable<Event<T>> events);
    Task Update<T>(Event<T> newEventData);
    Task<Event<T>> Get<T>(Guid eventId);
    Task<Event<T>> GetNextPending<T>(string? referenceId = null);
    Task<IEnumerable<Event<T>>> GetByState<T>(params EventState[] states);
}

public class EventRepository : IEventRepository
{
    public async Task Add<T>(Event<T> eventData)
    {
        throw new NotImplementedException();
    }

    public async Task Add<T>(IEnumerable<Event<T>> events)
    {
        throw new NotImplementedException();
    }

    public async Task Update<T>(Event<T> newEventData)
    {
        throw new NotImplementedException();
    }

    public async Task<Event<T>> Get<T>(Guid eventId)
    {
        throw new NotImplementedException();
    }

    public async Task<Event<T>> GetNextPending<T>(string? referenceId = null)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Event<T>>> GetByState<T>(params EventState[] states)
    {
        throw new NotImplementedException();
    }
}