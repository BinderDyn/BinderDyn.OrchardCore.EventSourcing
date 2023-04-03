using BinderDyn.OrchardCore.EventSourcing.Services;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Configuration;
using YesSql;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventTableManager
{
    Task CreateTableIfNotExist();
} 

public class EventTableManager : IEventTableManager
{
    
    
    private readonly ISession _session;
    private readonly IEventTableNameService _eventTableNameService;

    public EventTableManager(ISession session, 
        IEventTableNameService eventTableNameService)
    {
        _session = session;
        _eventTableNameService = eventTableNameService;
    }

    public async Task CreateTableIfNotExist()
    {
        var tableName = _eventTableNameService.CreateTableNameWithPrefixOrWithout();
        await _session.Store.InitializeCollectionAsync(tableName);
        await _session.Store.InitializeAsync();
    }
}