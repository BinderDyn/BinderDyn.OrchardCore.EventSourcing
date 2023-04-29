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
    private readonly Mock<IEventAccessService> _eventAccessServiceMock;
    private readonly Mock<ILogger<EventController>> _loggerMock;

    private readonly EventController _sut;

    public EventControllerTests()
    {
        _eventAccessServiceMock = new Mock<IEventAccessService>();
        _loggerMock = new Mock<ILogger<EventController>>();

        _sut = new EventController(_eventAccessServiceMock.Object, _loggerMock.Object);
    }

    public class List : EventControllerTests
    {
        [Fact]
        public async Task ReturnsView()
        {
            var result = await _sut.Index();

            result.Should().BeOfType<ViewResult>();
        }

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

            var result = await _sut.List(It.IsAny<EventFilter>());

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.As<EventViewModel[]>().Length.Should().Be(4);
        }

        [Fact]
        public async Task ReturnsEmptyListOnFailure()
        {
            _eventAccessServiceMock.Setup(m => m.GetAllFiltered(It.IsAny<EventFilter>()))
                .ThrowsAsync(new Exception("boom"));

            var result = await _sut.List(It.IsAny<EventFilter>());

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.As<EventViewModel[]>().Length.Should().Be(0);
        }
    }
}