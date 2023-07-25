using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using YesSql;
using YesSql.Sql;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventTableManager
{
    Task CreateTableIfNotExist(ISchemaBuilder schemaBuilder);
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

    public async Task CreateTableIfNotExist(ISchemaBuilder schemaBuilder)
    {
        var tableName = _eventTableNameService.CreateTableNameWithPrefixOrWithout();
        schemaBuilder.CreateTable(tableName, table => table
            .Column<Guid>(nameof(Event.EventId))
            .Column<DateTime>(nameof(Event.CreatedUtc))
            .Column<DateTime?>(nameof(Event.ProcessedUtc))
            .Column<Guid?>(nameof(Event.OriginalEventId))
            .Column<string?>(nameof(Event.ReferenceId))
            .Column<string>(nameof(Event.Payload))
            .Column<string>(nameof(Event.PayloadType))
            .Column<string>(nameof(Event.EventTypeFriendlyName))
            .Column<EventState>(nameof(Event.EventState)));
    }
}