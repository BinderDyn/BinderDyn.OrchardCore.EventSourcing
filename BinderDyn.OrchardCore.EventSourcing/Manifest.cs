using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "BinderDyn.OrchardCore.EventSourcing",
    Author = "Marvin Binder",
    Website = "https://marvinbinder.com",
    Version = "1.2.0",
    Description = "Enables event storage, procession and monitoring",
    Category = "Event Sourcing",
    Dependencies = new[]
    {
        "OrchardCore.Localization",
        "BinderDyn.OrchardCore.EventSourcing.Abstractions",
        "BinderDyn.OrchardCore.EventSourcing.SqlServer",
        "BinderDyn.OrchardCore.EventSourcing.Postgres",
        "BinderDyn.OrchardCore.EventSourcing.MySql"
    }
)]