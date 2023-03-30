using BinderDyn.OrchardCore.EventSourcing.Enums;
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
        throw new NotImplementedException();
    }

    public async Task<Event<T>> Get<T>(Guid eventId)
    {
        // TODO: Requires index
        throw new NotImplementedException();
    }

    public async Task<Event<T>> GetNextPending<T>(string? referenceId = null)
    {
        // TODO: Requires index
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Event<T>>> GetByState<T>(params EventState[] states)
    {
        // TODO: Requires index
        throw new NotImplementedException();
    }
}