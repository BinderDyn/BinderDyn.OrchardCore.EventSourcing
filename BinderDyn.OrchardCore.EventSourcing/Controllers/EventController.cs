using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrchardCore.Admin;

namespace BinderDyn.OrchardCore.EventSourcing.Controllers;

[Admin]
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
    public async Task<ActionResult> Index()
    {
        return View("Index");
    }

    [HttpPost("list")]
    public async Task<ActionResult> List([FromBody] EventFilter filter)
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