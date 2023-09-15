using BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;
using BinderDyn.OrchardCore.TablePrefixInterception;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace BinderDyn.OrchardCore.EventSourcing.Abstractions;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IDbConnectionProvider, DbConnectionProvider>();
        services.AddTablePrefixInterceptor(options =>
        {
            options.TableNamesToPrefix = new[]
            {
                "Events"
            };
        });
    }
}