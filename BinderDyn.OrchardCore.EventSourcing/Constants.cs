using BinderDyn.OrchardCore.EventSourcing.Abstractions.Enums;

namespace BinderDyn.OrchardCore.EventSourcing;

public static class Constants
{
    public static EventState[] InvalidStatesFromFailed = new[]
    {
        EventState.Processed,
        EventState.Aborted,
        EventState.Pending
    };

    public static EventState[] InvalidStatesFromPending = new[]
    {
        EventState.Processed,
        EventState.Failed
    };

    public static EventState[] InvalidStatesFromProcessing = new[]
    {
        EventState.Pending
    };
}