using System.Collections;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;
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
    /// <param name="referenceId">This could be a content item id or something else for fetching all event data for one referenceId</param>
    /// <typeparam name="T">Any object type serializable to JSON</typeparam>
    /// <returns>The id of the stored event</returns>
    Task<Guid> Add<T>(T payload, string friendlyName, string? referenceId = null);
    /// <summary>
    /// Gets the oldest event pending from the event store
    /// </summary>
    /// <typeparam name="T">The type of the stored payload</typeparam>
    /// <returns></returns>
    Task<Event<T>> GetNextPending<T>(string? referenceId = null);
    Task SetInProcessing<T>(Guid eventId);
    Task SetAsProcessed<T>(Guid eventId);
    Task SetAsFailed<T>(Guid eventId);
    Task SetAsAborted<T>(Guid eventId);
}

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IClock _clock;
    private readonly IGuidWrapper _guidWrapper;
    private readonly IStateGuardService _stateGuardService;
    
    public EventService(IEventRepository eventRepository, 
        IClock clock, 
        IGuidWrapper guidWrapper, 
        IStateGuardService stateGuardService)
    {
        _eventRepository = eventRepository;
        _clock = clock;
        _guidWrapper = guidWrapper;
        _stateGuardService = stateGuardService;
    }

    public async Task<Guid> Add<T>(T payload, string friendlyName, string? referenceId = null)
    {
        if (payload == null) throw new NoEventDataProvidedException();
        var eventData = new Event<T>()
        {
            Created = _clock.UtcNow,
            Payload = payload,
            EventId = _guidWrapper.NewGuid(),
            PayloadType = typeof(T).ToString(),
            EventTypeFriendlyName = friendlyName
        };
        await _eventRepository.Add(eventData);

        return eventData.EventId;
    }

    public async Task<Event<T>> GetNextPending<T>(string? referenceId = null)
    {
        return await _eventRepository.GetNextPending<T>(referenceId);
    }

    public async Task SetInProcessing<T>(Guid eventId)
    {
        var eventData = await GetEventOrThrow<T>(eventId);
        _stateGuardService.AssertCanSetOrThrow(eventId, eventData.EventState, EventState.Processed);

        eventData.EventState = EventState.InProcessing;

        await _eventRepository.Update(eventData);
    }

    public async Task SetAsProcessed<T>(Guid eventId)
    {
        var eventData = await GetEventOrThrow<T>(eventId);
        _stateGuardService.AssertCanSetOrThrow(eventId, eventData.EventState, EventState.Processed);

        eventData.EventState = EventState.Processed;
        eventData.Processed = _clock.UtcNow;

        await _eventRepository.Update(eventData);
    }

    public async Task SetAsFailed<T>(Guid eventId)
    {
        var eventData = await GetEventOrThrow<T>(eventId);
        _stateGuardService.AssertCanSetOrThrow(eventId, eventData.EventState, EventState.Failed);

        eventData.EventState = EventState.Failed;

        await _eventRepository.Update(eventData);
    }

    public async Task SetAsAborted<T>(Guid eventId)
    {
        var eventData = await GetEventOrThrow<T>(eventId);
        _stateGuardService.AssertCanSetOrThrow(eventId, eventData.EventState, EventState.Aborted);

        eventData.EventState = EventState.Aborted;

        await _eventRepository.Update(eventData);
    }

    private async Task<Event<T>> GetEventOrThrow<T>(Guid eventId)
    {
        var eventData = await _eventRepository.Get<T>(eventId);
        if (eventData == null) throw new EventNotFoundException(eventId);
        return eventData;
    }
}