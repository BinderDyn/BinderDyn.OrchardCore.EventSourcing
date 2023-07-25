using BinderDyn.OrchardCore.EventSourcing.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BinderDyn.OrchardCore.EventSourcing.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void AutoMigrateDatabase(this IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var context = serviceScope.ServiceProvider.GetService<EventSourcingDbContext>();
        context?.Database.Migrate();
    }
}