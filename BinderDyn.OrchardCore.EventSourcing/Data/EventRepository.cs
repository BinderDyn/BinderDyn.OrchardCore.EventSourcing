using System.Data.Common;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;
using BinderDyn.OrchardCore.EventSourcing.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrchardCore.Data;
using OrchardCore.Environment.Shell;
using YesSql;
using YesSql.Commands;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventRepository
{
    Task<Guid> Add(Event.EventCreationParam param);
    Task Update(Event newEventData);
    Task<Event?> Get(Guid eventId);
    Task<Event?> GetNextPending(string? referenceId = null);
    Task<IEnumerable<Event>> GetPagedByStates(int skip = 0, int take = 30, params EventState[] states);
}

public class EventRepository : IEventRepository
{
    private readonly EventSourcingDbContext _dbContext;

    public EventRepository(EventSourcingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Add(Event.EventCreationParam? param)
    {
        if (param is null)
            throw new ArgumentNullException(nameof(param), "Event creation param cannot be null!");
        Event? originalEvent = default;
        if (param.OriginalEventId.HasValue)
        {
            originalEvent = await _dbContext.Events.SingleOrDefaultAsync(x => x.EventId == param.OriginalEventId);
        }

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

        _dbContext.Add(newEvent);

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
        return await _dbContext.Events
            .Where(x => states.Contains(x.EventState))
            .Skip(skip)
            .Take(take)
            .ToArrayAsync();
    }
}