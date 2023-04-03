using BinderDyn.EventSourcing.Example.Content;
using BinderDyn.OrchardCore.EventSourcing.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Handlers;

namespace BinderDyn.EventSourcing.Example.Handlers;

public class BookHandler : ContentPartHandler<Book>
{
    private readonly IServiceProvider _serviceProvider;

    public BookHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public override async Task PublishedAsync(PublishContentContext context, Book instance)
    {
        var eventService = _serviceProvider.GetRequiredService<IEventService>();

        await eventService.Add(instance.BookTitle.Text, "BookAddedEvent", instance.ContentItem.ContentItemId);
        
        await base.PublishedAsync(context, instance);
    }
}