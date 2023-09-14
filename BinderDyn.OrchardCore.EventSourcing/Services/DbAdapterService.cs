using BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;
using BinderDyn.OrchardCore.EventSourcing.MySql.Data;
using BinderDyn.OrchardCore.EventSourcing.Postgres.Data;
using BinderDyn.OrchardCore.EventSourcing.SqlServer.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BinderDyn.OrchardCore.EventSourcing.Services;

public interface IDbAdapterService
{
    IEventSourcingDbContext GetCorrectContext();
}

public class DbAdapterService : IDbAdapterService
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly IServiceProvider _serviceProvider;

    public DbAdapterService(IDbConnectionProvider dbConnectionProvider, 
        IServiceProvider serviceProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _serviceProvider = serviceProvider;
    }

    public IEventSourcingDbContext GetCorrectContext()
    {
        var dbProvider = _dbConnectionProvider.GetProvider();

        return dbProvider switch
        {
            "SqlConnection" => _serviceProvider.GetRequiredService<EventSourcingSqlDbContext>(),
            "Postgres" => _serviceProvider.GetRequiredService<EventSourcingPostgresDbContext>(),
            "MySql" => _serviceProvider.GetRequiredService<EventSourcingMySqlDbContext>(),
            _ => throw new NotImplementedException(dbProvider)
        };
    }
}