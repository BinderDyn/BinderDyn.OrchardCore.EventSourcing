using BinderDyn.OrchardCore.EventSourcing.Services;
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

        try
        {
            context?.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}