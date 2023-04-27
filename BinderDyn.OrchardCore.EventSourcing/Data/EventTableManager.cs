using BinderDyn.OrchardCore.EventSourcing.Indices;
using BinderDyn.OrchardCore.EventSourcing.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Builders;
using YesSql;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventTableManager
{
    Task CreateTableIfNotExist();
    void RegisterIndexes();
} 

public class EventTableManager : IEventTableManager
{
    
    
    private readonly ISession _session;
    private readonly IEventTableNameService _eventTableNameService;
    private readonly IServiceProvider _serviceProvider;

    public EventTableManager(ISession session, 
        IEventTableNameService eventTableNameService, 
        IServiceProvider serviceProvider)
    {
        _session = session;
        _eventTableNameService = eventTableNameService;
        _serviceProvider = serviceProvider;
    }

    public async Task CreateTableIfNotExist()
    {
        var tableName = _eventTableNameService.CreateTableNameWithPrefixOrWithout();
        await _session.Store.InitializeCollectionAsync(tableName);
    }

    public void RegisterIndexes()
    {
        var tableName = _eventTableNameService.CreateTableNameWithPrefixOrWithout();
        
        EventIndexProvider GetIndexProvider()
        {
            try
            {
                return _serviceProvider.GetRequiredService<EventIndexProvider>();
            }
            catch (Exception)
            {
                return _serviceProvider.CreateInstance<EventIndexProvider>();
            }
        }

        _session.Store.RegisterIndexes(new[]
        {
            GetIndexProvider()
        }, tableName);
    }
}