namespace BinderDyn.OrchardCore.EventSourcing.Exceptions;

public class EventAlreadyFailedException : Exception
{
    public EventAlreadyFailedException(Guid id) : base($"Event with id {id} already failed!")
    {
        
    }
}