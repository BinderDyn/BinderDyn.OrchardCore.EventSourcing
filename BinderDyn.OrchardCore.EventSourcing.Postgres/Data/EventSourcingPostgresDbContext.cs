﻿using BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;

namespace BinderDyn.OrchardCore.EventSourcing.Postgres.Data;

#pragma warning disable CS8618

public class EventSourcingPostgresDbContext : DbContext, IEventSourcingDbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Action<DbContextOptionsBuilder>? _overrideOnConfiguring;

    public EventSourcingPostgresDbContext(IServiceProvider serviceProvider,
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
            optionsBuilder.UseNpgsql("");
            return;
        }

        // Used in production/development
        optionsBuilder.UseNpgsql(connectionString, options => options.CommandTimeout(600));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var shellSettings = _serviceProvider.GetRequiredService<ShellSettings>();
        var tablePrefix = shellSettings["TablePrefix"];

        if (!string.IsNullOrWhiteSpace(tablePrefix))
            modelBuilder.Entity<Event>().ToTable(tablePrefix + "_" + "Events");

        base.OnModelCreating(modelBuilder);
    }

    public virtual DbSet<Event> Events { get; set; }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}