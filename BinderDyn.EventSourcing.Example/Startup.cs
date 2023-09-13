using BinderDyn.EventSourcing.Example.Content;
using BinderDyn.EventSourcing.Example.Handlers;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;

namespace BinderDyn.EventSourcing.Example;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services
            .AddContentPart<Book>()
            .AddHandler<BookHandler>();
        services.AddScoped<IDataMigration, Migrations>();
    }
}