using BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;

namespace BinderDyn.OrchardCore.EventSourcing.MySql.Data;

#pragma warning disable CS8618

public class EventSourcingMySqlDbContext : DbContext, IEventSourcingDbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Action<DbContextOptionsBuilder>? _overrideOnConfiguring;

    public EventSourcingMySqlDbContext(IServiceProvider serviceProvider,
        Action<DbContextOptionsBuilder>? overrideOnConfiguring = null)
    {
        _serviceProvider = serviceProvider;
        _overrideOnConfiguring = overrideOnConfiguring;
    }

    private IDbConnectionProvider? DbConnectionProvider =>
        _serviceProvider.GetService<IDbConnectionProvider>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Used in tests
        if (_overrideOnConfiguring != null)
        {
            _overrideOnConfiguring(optionsBuilder);
            return;
        }

        // Used for adding migrations via cli
        var connectionString = DbConnectionProvider?.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseMySQL("REPLACE_WITH_VALID_CONNECTION_STRING_IN_DEVELOPMENT");
            return;
        }

        // Used in production/development
        optionsBuilder.UseMySQL(connectionString, options => options.CommandTimeout(600));
        optionsBuilder.AddInterceptors(_serviceProvider.GetRequiredService<TablePrefixInterceptor>());
    }

    public virtual DbSet<Event> Events { get; set; }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}