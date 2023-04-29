using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BinderDyn.OrchardCore.EventSourcing.Controllers;

public class EventController : Controller
{
    private readonly IEventAccessService _eventAccessService;
    private readonly ILogger<EventController> _logger;

    public EventController(IEventAccessService eventAccessService,
        ILogger<EventController> logger)
    {
        _eventAccessService = eventAccessService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> List(EventFilter filter)
    {
        try
        {
            return Ok(await _eventAccessService.GetAllFiltered(filter));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not get all filtered events");
            return Ok(Array.Empty<EventViewModel>());
        }
    }
}