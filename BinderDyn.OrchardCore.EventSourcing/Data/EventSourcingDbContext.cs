using BinderDyn.OrchardCore.EventSourcing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

#pragma warning disable CS8618

public class EventSourcingDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Action<DbContextOptionsBuilder>? _overrideOnConfiguring;

    public EventSourcingDbContext(IServiceProvider serviceProvider,
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
            optionsBuilder.UseSqlServer("");
            return;
        }

        // Used in production/development
        optionsBuilder.UseSqlServer(connectionString, options => options.CommandTimeout(600));
    }

    public virtual DbSet<Event> Events { get; set; }
}