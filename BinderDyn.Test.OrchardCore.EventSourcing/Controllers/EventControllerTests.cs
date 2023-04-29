using System;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Controllers;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Controllers;

public class EventControllerTests
{
    private readonly EventController _sut;

    public EventControllerTests()
    {
        _sut = new EventController();
    }

    
}