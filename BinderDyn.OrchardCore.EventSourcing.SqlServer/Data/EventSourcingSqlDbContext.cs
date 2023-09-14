using BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BinderDyn.OrchardCore.EventSourcing.SqlServer.Data;

#pragma warning disable CS8618

public class EventSourcingSqlDbContext : DbContext, IEventSourcingDbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _tablePrefix = string.Empty;
    private readonly Action<DbContextOptionsBuilder>? _overrideOnConfiguring;

    public EventSourcingSqlDbContext(IServiceProvider serviceProvider, string tablePrefix,
        Action<DbContextOptionsBuilder>? overrideOnConfiguring = null)
    {
        _serviceProvider = serviceProvider;
        _tablePrefix = tablePrefix;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>().ToTable(_tablePrefix + "_" + "Events");

        base.OnModelCreating(modelBuilder);
    }

    public virtual DbSet<Event> Events { get; set; }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}