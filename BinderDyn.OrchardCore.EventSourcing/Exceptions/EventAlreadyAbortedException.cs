namespace BinderDyn.OrchardCore.EventSourcing.Exceptions;

public class EventAlreadyAbortedException : Exception
{
    public EventAlreadyAbortedException(Guid id) : base($"Event with id {id} already aborted!")
    {
    }
}