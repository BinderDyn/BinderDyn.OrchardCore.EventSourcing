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
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        return View("Index");
    }
}