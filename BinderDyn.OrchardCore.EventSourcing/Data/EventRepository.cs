using BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Enums;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;
using BinderDyn.OrchardCore.EventSourcing.Services;
using Microsoft.EntityFrameworkCore;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventRepository
{
    Task<Guid> Add(Event.EventCreationParam param);
    Task Update(Event newEventData);
    Task<Event?> Get(Guid eventId);
    Task<Event?> GetNextPending(string? referenceId = null);
    Task<IEnumerable<Event>> GetPagedByStates(int skip = 0, int take = 30, params EventState[] states);
    Task<int> GetCountOfEventsForState(EventState state);
}

public class EventRepository : IEventRepository
{
    private readonly IEventSourcingDbContext _dbContext;

    public EventRepository(IDbAdapterService dbAdapterService)
    {
        _dbContext = dbAdapterService.GetCorrectContext();
    }

    public async Task<Guid> Add(Event.EventCreationParam? param)
    {
        if (param is null)
            throw new ArgumentNullException(nameof(param), "Event creation param cannot be null!");
        Event? originalEvent = default;
        if (param.OriginalEventId.HasValue)
            originalEvent = await _dbContext.Events.SingleOrDefaultAsync(x => x.EventId == param.OriginalEventId);

        var newEvent = new Event()
        {
            EventState = EventState.Pending,
            OriginalEvent = originalEvent,
            CreatedUtc = DateTime.UtcNow,
            ReferenceId = param.ReferenceId,
            Payload = param.Payload,
            EventTypeFriendlyName = param.EventTypeFriendlyName,
            PayloadType = param.PayloadType
        };

        _dbContext.Events.Add(newEvent);

        await _dbContext.SaveChangesAsync();

        return newEvent.EventId;
    }

    public async Task Update(Event newEventData)
    {
        try
        {
            var oldEvent = await _dbContext.Events.SingleAsync(x => x.EventId == newEventData.EventId);

            oldEvent.Update(newEventData);

            await _dbContext.SaveChangesAsync();
        }
        catch (InvalidOperationException)
        {
            throw new EventNotFoundException(newEventData.EventId);
        }
    }

    public async Task<Event> Get(Guid eventId)
    {
        try
        {
            return await _dbContext.Events.SingleAsync(x => x.EventId == eventId);
        }
        catch (InvalidOperationException)
        {
            throw new EventNotFoundException(eventId);
        }
    }

    public async Task<Event?> GetNextPending(string? referenceId = null)
    {
        var query = _dbContext.Events
            .Where(x => x.EventState == EventState.Pending)
            .OrderByDescending(x => x.CreatedUtc);

        if (referenceId is not null)
            return await query.FirstOrDefaultAsync(x => x.ReferenceId == referenceId);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Event>> GetPagedByStates(int skip = 0, int take = 30, params EventState[] states)
    {
        var query = _dbContext.Events.AsQueryable();

        if (states.Contains(EventState.All))
            return await query
                .Skip(skip)
                .Take(take)
                .ToArrayAsync();

        return await query.Where(x => states.Contains(x.EventState))
            .Skip(skip)
            .Take(take)
            .ToArrayAsync();
    }

    public async Task<int> GetCountOfEventsForState(EventState state)
    {
        if (state == EventState.All) return await _dbContext.Events.CountAsync();
        return await _dbContext.Events.CountAsync(e => e.EventState == state);
    }
}