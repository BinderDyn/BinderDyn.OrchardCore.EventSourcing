using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using OrchardCore.Data;
using YesSql.Indexes;
using YesSql.Sql;

namespace BinderDyn.OrchardCore.EventSourcing.Indices;

public class EventIndex : MapIndex
{
    public string EventId { get; set; }
    public string? ReferenceId { get; set; }
    public string? OriginalEventId { get; set; }
    public EventState EventState { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Processed { get; set; }
    public string PayloadType { get; set; }

    public static void WithSchemaBuilder(ISchemaBuilder schemaBuilder, IEventTableNameService eventTableNameService)
    {
        schemaBuilder.CreateMapIndexTable<EventIndex>(table => table
            .Column<string>(nameof(EventId))
            .Column<string?>(nameof(OriginalEventId))
            .Column<string?>(nameof(ReferenceId))
            .Column<EventState>(nameof(EventState))
            .Column<DateTime>(nameof(Created))
            .Column<DateTime?>(nameof(Processed))
            .Column<string>(nameof(PayloadType)), 
            eventTableNameService.CreateTableNameWithPrefixOrWithout());

        schemaBuilder.AlterIndexTable<EventIndex>(table => table
            .CreateIndex("IDX_EventIndex_Document", 
                nameof(EventId),
                nameof(OriginalEventId),
                nameof(ReferenceId),
                nameof(EventState),
                nameof(Created),
                nameof(Processed),
                nameof(PayloadType)), eventTableNameService.CreateTableNameWithPrefixOrWithout());
    }
}

public class EventIndexProvider : IndexProvider<Event>, IScopedIndexProvider
{
    public string CollectionName { get; set; } = "EventTable";

    public Type ForType()
    {
        return typeof(Event);
    }

    public override void Describe(DescribeContext<Event> context)
    {
        context.For<EventIndex>().Map(ei => new EventIndex()
        {
            EventId = ei.EventId.ToString(),
            ReferenceId = ei.ReferenceId,
            EventState = ei.EventState,
            OriginalEventId = ei.OriginalEventId.ToString(),
            Created = ei.Created,
            Processed = ei.Processed,
            PayloadType = ei.PayloadType
        });
    }
}