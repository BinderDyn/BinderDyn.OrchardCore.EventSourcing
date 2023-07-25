using System.Data.Common;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using Dapper;
using OrchardCore.Data;
using OrchardCore.Environment.Shell;
using YesSql;
using YesSql.Commands;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IEventRepository
{
    Task Add(Event? eventData);
    Task Update(Event newEventData);
    Task<Event> Get(Guid eventId);
    Task<Event> GetNextPending(string? referenceId = null);
    Task<IEnumerable<Event>> GetPagedByStates(int skip = 0, int take = 30, params EventState[] states);
}

public class EventRepository : IEventRepository
{
    private readonly IDbConnectionAccessor _dbConnectionAccessor;
    private readonly IStore _store;
    private readonly string _tablePrefix;


    public EventRepository(IDbConnectionAccessor dbConnectionAccessor, IStore store, ShellSettings settings)
    {
        _dbConnectionAccessor = dbConnectionAccessor;
        _store = store;
        _tablePrefix = settings["TablePrefix"];
    }

    public async Task Add(Event? eventData)
    {
        var eventId = Guid.NewGuid();
        using (var connection = _dbConnectionAccessor.CreateConnection())
        {
            await connection.OpenAsync();
            var dialect = _store.Configuration.SqlDialect;
            var customTable = dialect.QuoteForTableName($"{_tablePrefix}EventTable", _store.Configuration.Schema);
            var dbCommand = connection.CreateCommand();


            dbCommand.AddParameter("CreatedUtc", eventData.CreatedUtc.ToString());
            dbCommand.AddParameter("EventId", eventId.ToString());
            dbCommand.AddParameter("OriginalEventId", eventData.OriginalEventId.ToString());
            dbCommand.AddParameter("ReferenceId", eventData.ReferenceId);
            dbCommand.AddParameter("Payload", eventData.Payload);
            dbCommand.AddParameter("PayloadType", eventData.PayloadType);
            dbCommand.AddParameter("EventTypeFriendlyName", eventData.EventTypeFriendlyName);
            dbCommand.AddParameter("EventState", eventData.EventState.ToString());


            var insertSql = "INSERT INTO " + customTable + @" 
                (EventId, 
                 CreatedUtc,
                 OriginalEventId, 
                 ReferenceId, 
                 Payload,
                 PayloadType,
                 EventTypeFriendlyName,
                 EventState)
                 VALUES
                 EventId,
                 CreatedUtc,
                 OriginalEventId,
                 ReferenceId,
                 Payload,
                 PayloadType,
                 EventTypeFriendlyName,
                 EventState";

            dbCommand.CommandText = insertSql;

            dbCommand.ExecuteNonQuery();

            // If an exception occurs the transaction is disposed and rolled back
            connection.Close();
        }
    }

    public Task Update(Event newEventData)
    {
        throw new NotImplementedException();
    }

    public Task<Event> Get(Guid eventId)
    {
        throw new NotImplementedException();
    }

    public Task<Event> GetNextPending(string? referenceId = null)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Event>> GetPagedByStates(int skip = 0, int take = 30, params EventState[] states)
    {
        throw new NotImplementedException();
    }
}