namespace BinderDyn.OrchardCore.EventSourcing.Exceptions;

public class EventAlreadyProcessedException : Exception
{
    public EventAlreadyProcessedException(Guid id) : base($"Event with id {id} already processed!")
    {
        
    }
}