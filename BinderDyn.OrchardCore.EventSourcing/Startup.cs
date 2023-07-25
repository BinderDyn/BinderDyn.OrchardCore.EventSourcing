using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Extensions;
using BinderDyn.OrchardCore.EventSourcing.Services;
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
        services.AddScoped<IEventTableNameService, EventTableNameService>();
        services.AddScoped<IStateGuardService, StateGuardService>();
        services.AddScoped<IEventTableManager, EventTableManager>();
        services.AddScoped<IDbConnectionProvider, DbConnectionProvider>();
        services.AddScoped<IGuidWrapper, GuidWrapper>();
        services.AddScoped<INavigationProvider, AdminMenu>();
        services.AddScoped<IEventAccessService, EventAccessService>();

        services.AddDbContext<EventSourcingDbContext>();
    }
    
    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider)
    {
        builder.AutoMigrateDatabase();
    }
}