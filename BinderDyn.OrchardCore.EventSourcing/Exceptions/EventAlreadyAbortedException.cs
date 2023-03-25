namespace BinderDyn.OrchardCore.EventSourcing.Exceptions;

public class EventAlreadyAbortedExcpetion : Exception
{
    public EventAlreadyAbortedExcpetion(Guid id) : base($"Event with id {id} already aborted!")
    {
        
    }
}