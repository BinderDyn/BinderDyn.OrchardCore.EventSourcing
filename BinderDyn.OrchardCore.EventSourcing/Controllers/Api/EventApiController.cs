using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrchardCore.Admin;

namespace BinderDyn.OrchardCore.EventSourcing.Controllers.Api;

[ApiController]
[Route("api/events")]
[Admin]
public class EventApiController : ControllerBase
{
    private readonly IEventAccessService _eventAccessService;
    private readonly ILogger<EventController> _logger;

    public EventApiController(IEventAccessService eventAccessService, ILogger<EventController> logger)
    {
        _eventAccessService = eventAccessService;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<EventViewModel[]> List(int skip, int take, EventState states)
    {
        try
        {
            return await _eventAccessService.GetAllFiltered(new EventFilter()
            {
                Skip = skip,
                States = new[] {states},
                Take = take
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not get all filtered events");
            return Array.Empty<EventViewModel>();
        }
    }
}