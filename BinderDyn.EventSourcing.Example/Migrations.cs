using BinderDyn.EventSourcing.Example.Content;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace BinderDyn.EventSourcing.Example;

public class Migrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public Migrations(IContentDefinitionManager contentDefinitionManager)
    {
        _contentDefinitionManager = contentDefinitionManager;
    }
    
    public async Task<int> CreateAsync()
    {
        _contentDefinitionManager.AlterTypeDefinition(nameof(Book), builder => builder
            .WithPart(nameof(Book))
            .Listable()
            .Creatable());
        
        _contentDefinitionManager.AlterPartDefinition(nameof(Book), part => part
            .WithField(nameof(Book.BookTitle), field => field
                .OfType(nameof(TextField))
                .WithDisplayName("Book Title")));

        return 1;
    }
}