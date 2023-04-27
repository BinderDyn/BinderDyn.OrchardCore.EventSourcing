using System.Diagnostics.Eventing.Reader;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Indices;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.Wrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;

namespace BinderDyn.OrchardCore.EventSourcing
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IEventTableNameService, EventTableNameService>();
            services.AddScoped<IStateGuardService, StateGuardService>();
            services.AddScoped<IEventTableManager, EventTableManager>();
            services.AddScoped<IDataMigration, Migrations>();
            services.AddSingleton<IScopedIndexProvider, EventIndexProvider>();
            services.AddScoped<IGuidWrapper, GuidWrapper>();
        }
    }
}