namespace BinderDyn.OrchardCore.EventSourcing.Enums;

public enum EventState
{
    All = -1,
    Pending = 0,
    InProcessing = 1,
    Processed = 2,
    Aborted = 3,
    Failed = 4
}