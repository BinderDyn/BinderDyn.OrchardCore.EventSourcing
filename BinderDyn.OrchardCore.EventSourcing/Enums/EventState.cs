namespace BinderDyn.OrchardCore.EventSourcing.Enums;

public enum EventState
{
    Pending = 0,
    InProcessing = 1,
    Processed = 2,
    Aborted = 3,
    Failed = 4
}