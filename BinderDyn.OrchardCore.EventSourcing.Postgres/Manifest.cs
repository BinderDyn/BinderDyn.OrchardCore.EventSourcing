using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "BinderDyn.OrchardCore.EventSourcing.Postgres",
    Author = "Marvin Binder",
    Website = "https://marvinbinder.com",
    Version = "1.2.0",
    Description = "BinderDyn.OrchardCore.EventSourcing.Postgres",
    Category = "Event Sourcing",
    EnabledByDependencyOnly = true,
    Dependencies = new []
    {
        "BinderDyn.OrchardCore.EventSourcing.Abstractions"
    }
)]