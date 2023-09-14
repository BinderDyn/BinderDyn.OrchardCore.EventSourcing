using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using OrchardCore.Environment.Shell;
using OrchardCore.Settings;

namespace BinderDyn.OrchardCore.EventSourcing.SqlServer.Data;

public class SqlDbContextFactory : IDbContextFactory<EventSourcingSqlDbContext>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ShellSettings _shellSettings;

    public SqlDbContextFactory(IServiceProvider serviceProvider, ShellSettings shellSettings)
    {
        _serviceProvider = serviceProvider;
        _shellSettings = shellSettings;
    }

    public EventSourcingSqlDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventSourcingSqlDbContext>();
        var connectionString = _shellSettings["ConnectionString"];
        optionsBuilder.UseSqlServer(connectionString);

        return new EventSourcingSqlDbContext(_serviceProvider, _shellSettings["TablePrefix"]);
    }
}