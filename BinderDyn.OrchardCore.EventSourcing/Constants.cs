using BinderDyn.OrchardCore.EventSourcing.Enums;

namespace BinderDyn.OrchardCore.EventSourcing;

public static class Constants
{
    public static EventState[] InvalidStatesForSettingAsProcessed = new[]
    {
        EventState.Aborted,
        EventState.Failed,
        EventState.Processed
    };
}