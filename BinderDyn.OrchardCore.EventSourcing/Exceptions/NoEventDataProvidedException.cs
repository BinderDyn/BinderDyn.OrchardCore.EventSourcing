namespace BinderDyn.OrchardCore.EventSourcing.Exceptions;

public class NoEventDataProvidedException : Exception
{
    public NoEventDataProvidedException() : base("No event data provided!")
    {
        
    }
}