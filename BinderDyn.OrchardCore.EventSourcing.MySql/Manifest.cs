using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "BinderDyn.OrchardCore.EventSourcing.MySql",
    Author = "Marvin Binder",
    Website = "https://marvinbinder.com",
    Version = "1.1.0",
    Description = "BinderDyn.OrchardCore.EventSourcing.MySql",
    Category = "Event Sourcing",
    EnabledByDependencyOnly = true,
    Dependencies = new []
    {
        "BinderDyn.OrchardCore.EventSourcing.Abstractions"
    }
)]