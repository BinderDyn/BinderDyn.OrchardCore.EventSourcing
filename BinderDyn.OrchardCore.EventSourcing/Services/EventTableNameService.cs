using OrchardCore.Environment.Shell.Configuration;

namespace BinderDyn.OrchardCore.EventSourcing.Services;

public interface IEventTableNameService
{
    string CreateTableNameWithPrefixOrWithout();
}

public class EventTableNameService : IEventTableNameService
{
    private const string EventTableName = "EventTable";
    
    private readonly IShellConfiguration _shellConfiguration;

    public EventTableNameService(IShellConfiguration shellConfiguration)
    {
        _shellConfiguration = shellConfiguration;
    }

    public string CreateTableNameWithPrefixOrWithout()
    {
        var tablePrefix = _shellConfiguration["TablePrefix"];
        string tableName = EventTableName;
        if (!string.IsNullOrEmpty(tablePrefix))
        {
            tableName = $"{tablePrefix}_{EventTableName}";    
        }

        return tableName;
    }
}