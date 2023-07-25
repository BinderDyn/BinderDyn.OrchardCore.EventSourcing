using System.Data.Common;
using BinderDyn.OrchardCore.EventSourcing.Enums;
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
    Task Add(Event? eventData);
    Task Update(Event newEventData);
    Task<Event?> Get(Guid eventId);
    Task<Event?> GetNextPending(string? referenceId = null);
    Task<IEnumerable<Event>> GetPagedByStates(int skip = 0, int take = 30, params EventState[] states);
}

public class EventRepository : IEventRepository
{
    private readonly EventSourcingDbContext _dbContext;

    public async Task Add(Event? eventData)
    {
        _dbContext.Add(eventData);

        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Event newEventData)
    {
        var oldEvent = await _dbContext.Events.SingleAsync(x => x.EventId == newEventData.EventId);

        oldEvent.Update(newEventData);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Event> Get(Guid eventId)
    {
        return await _dbContext.Events.SingleAsync(x => x.EventId == eventId);
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