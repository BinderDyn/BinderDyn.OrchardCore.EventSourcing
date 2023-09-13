using BinderDyn.OrchardCore.EventSourcing.Abstractions.Enums;

namespace BinderDyn.OrchardCore.EventSourcing.Exceptions;

public class InvalidEventStateException : Exception
{
    public InvalidEventStateException(Guid id, EventState state) : base(
        $"Cannot set event with id {id} to pending from state {state}!")
    {
    }
}