using BinderDyn.OrchardCore.EventSourcing.Data;
using OrchardCore.Data.Migration;

namespace BinderDyn.OrchardCore.EventSourcing;

public class Migrations : DataMigration
{
    private readonly IEventTableManager _eventTableManager;

    public Migrations(IEventTableManager eventTableManager)
    {
        _eventTableManager = eventTableManager;
    }

    public async Task<int> CreateAsync()
    {
        await _eventTableManager.CreateTableIfNotExist(SchemaBuilder);

        return 1;
    }
}