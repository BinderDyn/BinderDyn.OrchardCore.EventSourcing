using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Models;

namespace BinderDyn.OrchardCore.EventSourcing.Services;

public interface IEventService
{
    /// <summary>
    /// Adds an event with the given payload to the event store
    /// </summary>
    /// <param name="payload"></param>
    /// <typeparam name="T">Any object type serializable to JSON</typeparam>
    /// <returns>The id of the stored event</returns>
    Task<Guid> Add<T>(T payload);
    /// <summary>
    /// Adds multiple events with given payloads in the order of the enumeration to the event store
    /// </summary>
    /// <param name="payloads">The payloads for storing in the event store</param>
    /// <typeparam name="T">Any object type serializable to JSON</typeparam>
    /// <returns>The ids of the stored events in the order of the given input</returns>
    Task<IEnumerable<Guid>> Add<T>(IEnumerable<T> payloads);
    /// <summary>
    /// Gets the oldest event pending from the event store
    /// </summary>
    /// <typeparam name="T">The type of the stored payload</typeparam>
    /// <returns></returns>
    Task<Event<T>> GetNextPending<T>();

    Task SetAsProcessed(Guid eventId);
    Task SetAsFailed(Guid eventId);
    Task SetAsAborted(Guid eventId);
}

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<Guid> Add<T>(T payload)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Guid>> Add<T>(IEnumerable<T> payloads)
    {
        throw new NotImplementedException();
    }

    public async Task<Event<T>> GetNextPending<T>()
    {
        throw new NotImplementedException();
    }

    public async Task SetAsProcessed(Guid eventId)
    {
        throw new NotImplementedException();
    }

    public async Task SetAsFailed(Guid eventId)
    {
        throw new NotImplementedException();
    }

    public async Task SetAsAborted(Guid eventId)
    {
        throw new NotImplementedException();
    }
}