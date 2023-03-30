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
    private const string EventTableName = "EventTable";
    
    private readonly ISession _session;
    private readonly IShellConfiguration _shellConfiguration;

    public EventTableManager(ISession session, IShellConfiguration shellConfiguration)
    {
        _session = session;
        _shellConfiguration = shellConfiguration;
    }

    public async Task CreateTableIfNotExist()
    {
        var tablePrefix = _shellConfiguration["TablePrefix"];
        string tableName = EventTableName;
        if (!string.IsNullOrEmpty(tablePrefix))
        {
            tableName = $"{tablePrefix}_{EventTableName}";    
        }
        
        await _session.Store.InitializeCollectionAsync(tableName);
    }
}