using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Indices;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using YesSql;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventRepository
{
    Task Add<T>(Event<T>? eventData);
    Task Update<T>(Event<T> newEventData);
    Task<Event<T>> Get<T>(Guid eventId);
    Task<Event<T>> GetNextPending<T>(string? referenceId = null);
    Task<IEnumerable<Event<T>>> GetByState<T>(params EventState[] states);
}

public class EventRepository : IEventRepository
{
    private readonly ISession _session;
    private readonly IEventTableNameService _eventTableNameService;

    public EventRepository(ISession session, 
        IEventTableNameService eventTableNameService)
    {
        _session = session;
        _eventTableNameService = eventTableNameService;
    }

    public async Task Add<T>(Event<T>? eventData)
    {
        if (eventData == null) return;
        var tableName = _eventTableNameService.CreateTableNameWithPrefixOrWithout();
        
        _session.Save(eventData, tableName);
        await _session.SaveChangesAsync();
    }

    public async Task Update<T>(Event<T> newEventData)
    {
        var oldEvent = await _session
            .Query<Event<T>, EventIndex>(q => q.EventId == newEventData.EventId.ToString())
            .FirstOrDefaultAsync();

        oldEvent = newEventData;
        _session.Save(oldEvent, _eventTableNameService.CreateTableNameWithPrefixOrWithout());

        await _session.SaveChangesAsync();
    }

    public async Task<Event<T>> Get<T>(Guid eventId)
    {
        return await _session
            .Query<Event<T>, EventIndex>(q => q.EventId == eventId.ToString(), 
                collection: _eventTableNameService.CreateTableNameWithPrefixOrWithout())
            .FirstOrDefaultAsync();
    }

    public async Task<Event<T>> GetNextPending<T>(string? referenceId = null)
    {
        var query = _session.Query<Event<T>, EventIndex>(q => 
            q.EventState == EventState.Pending, collection: _eventTableNameService.CreateTableNameWithPrefixOrWithout());
        if (!string.IsNullOrWhiteSpace(referenceId))
            query = query.Where(q => q.ReferenceId == referenceId);

        return await query.OrderBy(ev => ev.Created).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Event<T>>> GetByState<T>(params EventState[] states)
    {
        return await _session
            .Query<Event<T>, EventIndex>(q => states.Contains(q.EventState))
            .ListAsync();
    }
}