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
        switch (targetState)
        { 
            case EventState.Processed:
                if (Constants.InvalidStatesForSettingAsProcessed.Contains(currentState)) 
                    HandleErrorForSettingAsProcessed(eventId, currentState);
                break;
            default:
                throw new NotImplementedException();

        }
    }
    
    private void HandleErrorForSettingAsProcessed(Guid eventId, EventState curentState)
    {
        switch (curentState)
        {
            case EventState.Aborted:
                throw new EventAlreadyAbortedExcpetion(eventId);
            case EventState.Failed:
                throw new EventAlreadyFailedException(eventId);
            case EventState.Processed:
                throw new EventAlreadyProcessedException(eventId);
            default:
                throw new NotImplementedException(curentState.ToString());
        }
    }
}