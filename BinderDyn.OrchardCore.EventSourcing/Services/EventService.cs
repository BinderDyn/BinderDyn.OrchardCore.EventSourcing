using System.Collections;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Wrapper;
using OrchardCore.Modules;

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
    Task<IEnumerable<Guid>> Add<T>(params T[] payloads);
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
    private readonly IClock _clock;
    private readonly IGuidWrapper _guidWrapper;

    public EventService(IEventRepository eventRepository, 
        IClock clock, 
        IGuidWrapper guidWrapper)
    {
        _eventRepository = eventRepository;
        _clock = clock;
        _guidWrapper = guidWrapper;
    }

    public async Task<Guid> Add<T>(T payload)
    {
        var eventData = new Event<T>()
        {
            Created = _clock.UtcNow,
            Payload = payload,
            EventId = _guidWrapper.NewGuid(),
            PayloadType = typeof(T).ToString()
        };
        await _eventRepository.Add(eventData);

        return eventData.EventId;
    }

    public async Task<IEnumerable<Guid>> Add<T>(params T[] payloads)
    {
        var events = new List<Event<T>>();
        
        foreach (var payload in payloads)
        {
            var eventData = new Event<T>()
            {
                Created = _clock.UtcNow,
                Payload = payload,
                EventId = Guid.NewGuid(),
                PayloadType = typeof(T).ToString()
            };
            events.Add(eventData);
        }
        await _eventRepository.Add(events);

        return events.Select(e => e.EventId);
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