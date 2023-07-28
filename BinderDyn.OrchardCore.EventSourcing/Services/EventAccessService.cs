using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.ViewModels;
using Microsoft.Extensions.Logging;

namespace BinderDyn.OrchardCore.EventSourcing.Services;

public interface IEventAccessService
{
    Task<EventViewModel[]> GetAllFiltered(EventFilter filter);
    Task RescheduleEventForPending(Guid id);
    Task<int> GetCountOfEventsForState(EventState eventState);
}

public class EventAccessService : IEventAccessService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventAccessService> _logger;
    private readonly IStateGuardService _stateGuardService;

    public EventAccessService(IEventRepository eventRepository,
        ILogger<EventAccessService> logger,
        IStateGuardService stateGuardService)
    {
        _eventRepository = eventRepository;
        _logger = logger;
        _stateGuardService = stateGuardService;
    }

    public async Task<EventViewModel[]> GetAllFiltered(EventFilter filter)
    {
        try
        {
            var events = await _eventRepository.GetPagedByStates(filter.Skip, filter.Take, filter.States);
            return events.Select(e => new EventViewModel(e)).ToArray();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong getting all filtered events");
            throw;
        }
    }

    public async Task<int> GetCountOfEventsForState(EventState eventState)
    {
        return await _eventRepository.GetCountOfEventsForState(eventState);
    }

    public async Task RescheduleEventForPending(Guid id)
    {
        try
        {
            var currentEventData = await _eventRepository.Get(id);

            _stateGuardService.AssertCanSetOrThrow(id, currentEventData.EventState, EventState.Pending);
            currentEventData.EventState = EventState.Pending;

            await _eventRepository.Update(currentEventData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot set event with id {EventId} to pending state", id);
            throw;
        }
    }
}