using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Wrapper;
using Newtonsoft.Json;
using OrchardCore.Modules;

namespace BinderDyn.OrchardCore.EventSourcing.Services;

public interface IEventService
{
    /// <summary>
    /// Adds an event with the given payload to the event store
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="referenceId">This could be a content item id or something else for fetching all event data for one referenceId</param>
    /// <returns>The id of the stored event</returns>
    Task<Guid> Add(object payload, string friendlyName, string? referenceId = null, Guid? originalEventId = null);

    /// <summary>
    /// Gets the oldest event pending from the event store
    /// </summary>
    /// <returns></returns>
    Task<Event> GetNextPending(string? referenceId = null);

    Task SetInProcessing(Guid eventId);
    Task SetAsProcessed(Guid eventId);
    Task SetAsFailed(Guid eventId);
    Task SetAsAborted(Guid eventId);
}

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IClock _clock;
    private readonly IStateGuardService _stateGuardService;

    public EventService(IEventRepository eventRepository,
        IClock clock,
        IStateGuardService stateGuardService)
    {
        _eventRepository = eventRepository;
        _clock = clock;
        _stateGuardService = stateGuardService;
    }

    public async Task<Guid> Add(object payload, string friendlyName, string? referenceId = null,
        Guid? originalEventId = null)
    {
        if (payload == null) throw new NoEventDataProvidedException();
        var eventData = new Event.EventCreationParam()
        {
            Payload = JsonConvert.ToString(payload),
            PayloadType = payload.GetType().ToString(),
            EventTypeFriendlyName = friendlyName,
            ReferenceId = referenceId,
            OriginalEventId = originalEventId
        };
        var createdEventId = await _eventRepository.Add(eventData);

        return createdEventId;
    }

    public async Task<Event> GetNextPending(string? referenceId = null)
    {
        return await _eventRepository.GetNextPending(referenceId);
    }

    public async Task SetInProcessing(Guid eventId)
    {
        var eventData = await GetEventOrThrow(eventId);
        _stateGuardService.AssertCanSetOrThrow(eventId, eventData.EventState, EventState.Processed);

        eventData.EventState = EventState.InProcessing;

        await _eventRepository.Update(eventData);
    }

    public async Task SetAsProcessed(Guid eventId)
    {
        var eventData = await GetEventOrThrow(eventId);
        _stateGuardService.AssertCanSetOrThrow(eventId, eventData.EventState, EventState.Processed);

        eventData.EventState = EventState.Processed;
        eventData.ProcessedUtc = _clock.UtcNow;

        await _eventRepository.Update(eventData);
    }

    public async Task SetAsFailed(Guid eventId)
    {
        var eventData = await GetEventOrThrow(eventId);
        _stateGuardService.AssertCanSetOrThrow(eventId, eventData.EventState, EventState.Failed);

        eventData.EventState = EventState.Failed;

        await _eventRepository.Update(eventData);
    }

    public async Task SetAsAborted(Guid eventId)
    {
        var eventData = await GetEventOrThrow(eventId);
        _stateGuardService.AssertCanSetOrThrow(eventId, eventData.EventState, EventState.Aborted);

        eventData.EventState = EventState.Aborted;

        await _eventRepository.Update(eventData);
    }

    private async Task<Event> GetEventOrThrow(Guid eventId)
    {
        var eventData = await _eventRepository.Get(eventId);
        if (eventData == null) throw new EventNotFoundException(eventId);
        return eventData;
    }
}