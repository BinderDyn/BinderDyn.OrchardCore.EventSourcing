using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Indices;
using BinderDyn.OrchardCore.EventSourcing.Services;
using OrchardCore.Data.Migration;

namespace BinderDyn.OrchardCore.EventSourcing;

public class Migrations : DataMigration
{
    private readonly IEventTableManager _eventTableManager;
    private readonly IEventTableNameService _eventTableNameService;

    public Migrations(IEventTableManager eventTableManager, IEventTableNameService eventTableNameService)
    {
        _eventTableManager = eventTableManager;
        _eventTableNameService = eventTableNameService;
    }

    public async Task<int> CreateAsync()
    {
        await _eventTableManager.CreateTableIfNotExist();

        return 1;
    }

    public async Task<int> UpdateAsyncFrom1()
    {
        EventIndex.WithSchemaBuilder(SchemaBuilder, _eventTableNameService);

        return 2;
    }
}