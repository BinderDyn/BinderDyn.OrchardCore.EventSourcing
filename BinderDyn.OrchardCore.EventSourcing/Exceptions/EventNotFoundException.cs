namespace BinderDyn.OrchardCore.EventSourcing.Exceptions;

public class EventNotFoundException : Exception
{
    public EventNotFoundException(Guid id) : base($"No event for id {id}")
    {
        
    }
}