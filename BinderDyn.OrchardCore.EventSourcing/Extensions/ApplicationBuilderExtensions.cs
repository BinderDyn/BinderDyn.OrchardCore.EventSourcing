using BinderDyn.OrchardCore.EventSourcing.Postgres.Data;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.SqlServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BinderDyn.OrchardCore.EventSourcing.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void AutoMigrateDatabase(this IApplicationBuilder applicationBuilder, string provider)
    {
        using var serviceScope = applicationBuilder.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var dbAdapterService = serviceScope.ServiceProvider.GetRequiredService<IDbAdapterService>();
        var context = dbAdapterService.GetCorrectContext() as DbContext;

        context?.Database.Migrate();
    }
}