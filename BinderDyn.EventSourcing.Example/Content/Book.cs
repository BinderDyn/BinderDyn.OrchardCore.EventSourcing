using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace BinderDyn.EventSourcing.Example.Content;

public class Book : ContentPart
{
    public TextField BookTitle { get; set; }
}