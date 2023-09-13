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


        if (provider == "SqlConnection")
        {
            var context = serviceScope.ServiceProvider.GetService<EventSourcingSqlDbContext>();
            context?.Database.Migrate();
        }
    }
}