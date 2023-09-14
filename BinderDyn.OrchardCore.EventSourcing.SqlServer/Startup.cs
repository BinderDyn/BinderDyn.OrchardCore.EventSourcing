using BinderDyn.OrchardCore.EventSourcing.SqlServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace BinderDyn.OrchardCore.EventSourcing.SqlServer;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<EventSourcingSqlDbContext>();
        services.AddScoped<IDbContextFactory<EventSourcingSqlDbContext>, SqlDbContextFactory>();
    }
}