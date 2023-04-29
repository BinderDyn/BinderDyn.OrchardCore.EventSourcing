using BinderDyn.OrchardCore.EventSourcing.Enums;
using Newtonsoft.Json;

namespace BinderDyn.OrchardCore.EventSourcing.Models;

public class EventFilter
{
    [JsonProperty("states")] public EventState[] States { get; set; } = new[] {EventState.Pending};
    [JsonProperty("skip")] public int Skip { get; set; } = 0;
    [JsonProperty("take")] public int Take { get; set; } = 30;
}