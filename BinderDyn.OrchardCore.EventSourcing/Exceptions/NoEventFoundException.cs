namespace BinderDyn.OrchardCore.EventSourcing.Exceptions;

public class NoEventFoundException : Exception
{
    public NoEventFoundException(Guid eventId) : base($"No event with Id {eventId} found.")
    {
    }
}