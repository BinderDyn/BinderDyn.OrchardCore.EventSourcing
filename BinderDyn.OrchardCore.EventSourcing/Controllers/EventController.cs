using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using OrchardCore.Settings;

namespace BinderDyn.OrchardCore.EventSourcing.Controllers;

[Admin]
[IgnoreAntiforgeryToken]
public class EventController : Controller
{
    private readonly IEventAccessService _eventAccessService;
    private readonly ISiteService _siteService;

    public EventController(IEventAccessService eventAccessService, ISiteService siteService)
    {
        _eventAccessService = eventAccessService;
        _siteService = siteService;
    }

    [HttpGet]
    public async Task<ActionResult> Index(int page = 1, EventState state = EventState.All)
    {
        var eventCount = await _eventAccessService.GetCountOfEventsForState(state);
        var siteSettings = await _siteService.GetSiteSettingsAsync();
        var pageSize = siteSettings.PageSize;

        var paging = CalculatePaging(page, eventCount, pageSize);

        var events = await _eventAccessService.GetAllFiltered(new EventFilter()
        {
            Skip = paging.Skip,
            Take = pageSize,
            States = new[] {state}
        });

        return View("Index", new EventTableViewModel()
        {
            CurrentPage = page,
            PageSize = pageSize,
            Events = events,
            MaxPage = paging.AmountOfPages,
            BaseUrl = siteSettings.BaseUrl,
            State = state
        });
    }

    private Paging CalculatePaging(int currentPage, int allItems, int pageSize)
    {
        if (allItems == 0) allItems = 1;

        var pages = (int) Math.Ceiling((double) allItems / pageSize);

        return new Paging()
        {
            AmountOfPages = pages,
            Skip = (currentPage - 1) * pageSize,
        };
    }

    private class Paging
    {
        public int Skip { get; set; }
        public int AmountOfPages { get; set; }
    }
}