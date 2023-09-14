# BinderDyn.OrchardCore.EventSourcing
[![Build and test on main](https://github.com/BinderDyn/BinderDyn.OrchardCore.EventSourcing/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/BinderDyn/BinderDyn.OrchardCore.EventSourcing/actions/workflows/dotnet.yml)
[![Build and test on develop](https://github.com/BinderDyn/BinderDyn.OrchardCore.EventSourcing/actions/workflows/dotnet_develop.yml/badge.svg)](https://github.com/BinderDyn/BinderDyn.OrchardCore.EventSourcing/actions/workflows/dotnet_develop.yml)

This package is meant to be used with OrchardCore. List events, handle them and get a history in a relational table besides your other tables.

## Hint

This will work only with SQL-Server, Postgres and MySQL and with a unique database for each tenant. I didn't find a way to get this working with SQLite or with multiple tenants in one database.

## Usage

1. Install the package via nuget in the project that consumes the API
2. Add code that interacts with the event service. This is the main interface for storing, reading and processing the events.
   ```csharp
    // Example from the repository with a content part handler
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
   ```
3. Activate the feature in the tenant you want to use it for
4. You can view the created events in the 'Events' menu point in the admin frontend

## Create Events

Directly taken from the interface:
```csharp
Task<Guid> Add(object payload, string friendlyName, string? referenceId = null, Guid? originalEventId = null);
```
The payload can be any object as long as it is serializable. You can use referenceId to query via the event repository (e.g. for linking content items or storing other information).

## Get pending events

```csharp
// Gets the oldest event pending from the event store
Task<Event> GetNextPending(string? referenceId = null);
```

Again, you can use the referenceId for querying a specific event.

## Event states

```csharp
// Set the event state of a specific event
Task SetInProcessing(Guid eventId);
Task SetAsProcessed(Guid eventId);
Task SetAsFailed(Guid eventId);
Task SetAsAborted(Guid eventId);
```
