using System;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Controllers;
using BinderDyn.OrchardCore.EventSourcing.Controllers.Api;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Controllers.Api;

public class EventApiControllerTests
{
    private readonly Mock<IEventAccessService> _eventAccessServiceMock;
    private readonly Mock<ILogger<EventController>> _loggerMock;

    private readonly EventApiController _sut;

    public EventApiControllerTests()
    {
        _eventAccessServiceMock = new Mock<IEventAccessService>();
        _loggerMock = new Mock<ILogger<EventController>>();

        _sut = new EventApiController(_eventAccessServiceMock.Object, _loggerMock.Object);
    }

    public class List : EventApiControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkWithEvents()
        {
            _eventAccessServiceMock.Setup(m => m.GetAllFiltered(It.IsAny<EventFilter>()))
                .ReturnsAsync(new[]
                {
                    new EventViewModel(),
                    new EventViewModel(),
                    new EventViewModel(),
                    new EventViewModel()
                });

            var result = await _sut.List(0, 0, EventState.Pending);

            result.Length.Should().Be(4);
        }

        [Fact]
        public async Task ReturnsEmptyListOnFailure()
        {
            _eventAccessServiceMock.Setup(m => m.GetAllFiltered(It.IsAny<EventFilter>()))
                .ThrowsAsync(new Exception("boom"));

            var result = await _sut.List(0, 0, EventState.Pending);

            result.Length.Should().Be(0);
        }
    }
}