using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace BinderDyn.OrchardCore.EventSourcing;

public class AdminMenu : INavigationProvider
{
    private readonly IStringLocalizer<AdminMenu> _s;

    public AdminMenu(IStringLocalizer<AdminMenu> s)
    {
        _s = s;
    }

    public async Task BuildNavigationAsync(string name, NavigationBuilder builder)
    {
        builder.Add(_s["Menu.EventMenuPoint"], "0",
            builder =>
            {
                builder.Action("Index", "Event", "BinderDyn.OrchardCore.EventSourcing");
                builder.Id("events");
                builder.AddClass("events");
            });
    }
}