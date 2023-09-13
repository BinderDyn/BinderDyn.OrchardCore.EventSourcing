using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "BinderDyn.OrchardCore.EventSourcing.SqlServer",
    Author = "Marvin Binder",
    Website = "https://marvinbinder.com",
    Version = "1.1.0",
    Description = "BinderDyn.OrchardCore.EventSourcing.SqlServer",
    Category = "Event Sourcing",
    EnabledByDependencyOnly = true,
    Dependencies = new[]
    {
        "BinderDyn.OrchardCore.EventSourcing.Abstractions"
    }
)]