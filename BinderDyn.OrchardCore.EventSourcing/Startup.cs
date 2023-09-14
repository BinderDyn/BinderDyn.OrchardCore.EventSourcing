using BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Extensions;
using BinderDyn.OrchardCore.EventSourcing.MySql.Data;
using BinderDyn.OrchardCore.EventSourcing.Postgres.Data;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.SqlServer.Data;
using BinderDyn.OrchardCore.EventSourcing.Wrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using OrchardCore.Navigation;

namespace BinderDyn.OrchardCore.EventSourcing;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IStateGuardService, StateGuardService>();
        services.AddScoped<IGuidWrapper, GuidWrapper>();
        services.AddScoped<INavigationProvider, AdminMenu>();
        services.AddScoped<IEventAccessService, EventAccessService>();
        services.AddScoped<SqlDbContextFactory>();
        services.AddDbContext<EventSourcingPostgresDbContext>();
        services.AddDbContext<EventSourcingMySqlDbContext>();
        services.AddScoped<IDbAdapterService, DbAdapterService>();
    }

    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider)
    {
        var databaseProvider = serviceProvider.GetRequiredService<IDbConnectionProvider>();

        builder.AutoMigrateDatabase(databaseProvider?.GetProvider() ?? "");
    }
}