using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Indices;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using YesSql;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventRepository
{
    Task Add(Event? eventData);
    Task Update(Event newEventData);
    Task<Event> Get(Guid eventId);
    Task<Event> GetNextPending(string? referenceId = null);
    Task<IEnumerable<Event>> GetByState(params EventState[] states);
}

public class EventRepository : IEventRepository
{
    private readonly ISession _session;
    private readonly IEventTableNameService _eventTableNameService;
    private readonly IEventTableManager _eventTableManager;

    public EventRepository(ISession session, 
        IEventTableNameService eventTableNameService, 
        IEventTableManager eventTableManager)
    {
        _session = session;
        _eventTableNameService = eventTableNameService;
        _eventTableManager = eventTableManager;
    }

    private async Task EnsureInitialized()
    {
        await _eventTableManager.CreateTableIfNotExist();
    }

    public async Task Add(Event? eventData)
    {
        if (eventData == null) return;

        await EnsureInitialized();
        var tableName = _eventTableNameService.CreateTableNameWithPrefixOrWithout();
        
        _session.Save(eventData, tableName);
        await _session.SaveChangesAsync();
    }

    public async Task Update(Event newEventData)
    {
        await EnsureInitialized();
        
        var oldEvent = await _session
            .Query<Event, EventIndex>(q => q.EventId == newEventData.EventId.ToString())
            .FirstOrDefaultAsync();

        oldEvent = newEventData;
        _session.Save(oldEvent, _eventTableNameService.CreateTableNameWithPrefixOrWithout());

        await _session.SaveChangesAsync();
    }

    public async Task<Event> Get(Guid eventId)
    {
        await EnsureInitialized();
        
        return await _session
            .Query<Event, EventIndex>(q => q.EventId == eventId.ToString(), 
                collection: _eventTableNameService.CreateTableNameWithPrefixOrWithout())
            .FirstOrDefaultAsync();
    }

    public async Task<Event> GetNextPending(string? referenceId = null)
    {
        await EnsureInitialized();
        
        var query = _session.Query<Event, EventIndex>(q => 
            q.EventState == EventState.Pending, collection: _eventTableNameService.CreateTableNameWithPrefixOrWithout());
        if (!string.IsNullOrWhiteSpace(referenceId))
            query = query.Where(q => q.ReferenceId == referenceId);

        return await query.OrderBy(ev => ev.Created).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Event>> GetByState(params EventState[] states)
    {
        await EnsureInitialized();
        
        return await _session
            .Query<Event, EventIndex>(q => states.Contains(q.EventState))
            .ListAsync();
    }
}