using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;

namespace BinderDyn.OrchardCore.EventSourcing.Services;

public interface IStateGuardService
{
    void AssertCanSetOrThrow(Guid eventId, EventState currentState, EventState targetState);
}

public class StateGuardService : IStateGuardService
{
    public void AssertCanSetOrThrow(Guid eventId, EventState currentState, EventState targetState)
    {
        switch (currentState)
        {
            case EventState.Processed:
                throw new EventAlreadyProcessedException(eventId);
                break;
            case EventState.Aborted:
                throw new EventAlreadyAbortedException(eventId);
                break;
            case EventState.Failed:
                if (Constants.InvalidStatesFromFailed.Contains(targetState))
                    throw new EventAlreadyFailedException(eventId);
                break;
            case EventState.Pending:
                if (Constants.InvalidStatesFromPending.Contains(targetState))
                    throw new InvalidEventStateException(eventId, currentState);
                break;
            case EventState.InProcessing:
                if (Constants.InvalidStatesFromProcessing.Contains(targetState))
                    throw new InvalidEventStateException(eventId, currentState);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}