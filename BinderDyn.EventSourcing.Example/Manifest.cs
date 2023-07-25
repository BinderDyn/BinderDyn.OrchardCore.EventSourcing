using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "BinderDyn.EventSourcing.Example",
    Author = "Marvin Binder",
    Website = "https://marvinbinder.com",
    Version = "0.0.1",
    Description = "Example for implementing the event sourcing module",
    Category = "Event Sourcing",
    IsAlwaysEnabled = true,
    Dependencies = new [] { "BinderDyn.OrchardCore.EventSourcing", "OrchardCore.Contents" }
)]