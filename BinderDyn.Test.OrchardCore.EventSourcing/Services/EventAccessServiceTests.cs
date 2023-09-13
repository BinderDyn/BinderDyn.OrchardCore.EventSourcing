using System;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Enums;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Services;

public class EventAccessServiceTests
{
    private readonly IEventRepository _eventRepositoryMock;
    private readonly ILogger<EventAccessService> _loggerMock;
    private readonly IStateGuardService _stateGuardServiceMock;
    private readonly EventAccessService _sut;

    public EventAccessServiceTests()
    {
        _eventRepositoryMock = Substitute.For<IEventRepository>();
        _loggerMock = Substitute.For<ILogger<EventAccessService>>();
        _stateGuardServiceMock = Substitute.For<IStateGuardService>();

        _sut = new EventAccessService(_eventRepositoryMock, _loggerMock, _stateGuardServiceMock);
    }

    public class GetAllFiltered : EventAccessServiceTests
    {
        [Fact]
        public async Task ShouldReturnViewModelsFromEventsFetched()
        {
            _eventRepositoryMock
                .GetPagedByStates(0, 5, Arg.Any<EventState[]>())
                .Returns(new[]
                {
                    new Event(),
                    new Event(),
                    new Event()
                });

            var result = await _sut.GetAllFiltered(new EventFilter()
            {
                Skip = 0,
                Take = 5,
                States = new[] { EventState.Pending }
            });

            await _eventRepositoryMock.Received().GetPagedByStates(0, 5, new[] { EventState.Pending });

            result.Length.Should().Be(3);
        }

        [Fact]
        public async Task ReturnEmptyListIfNothingFoundForCriteria()
        {
            var result = await _sut.GetAllFiltered(new EventFilter()
            {
                Skip = 0,
                Take = 5,
                States = new[] { EventState.Pending }
            });

            await _eventRepositoryMock.Received().GetPagedByStates(0, 5, new[] { EventState.Pending });

            result.Length.Should().Be(0);
        }

        [Fact]
        public async Task ThrowsOnError()
        {
            _eventRepositoryMock
                .When(x => x.GetPagedByStates(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<EventState[]>()))
                .Do(x => throw new Exception());

            await Assert.ThrowsAsync<Exception>(async () => await _sut.GetAllFiltered(new EventFilter()));
        }
    }

    public class RescheduleEventForPending : EventAccessServiceTests
    {
        [Fact]
        public async Task FetchesEventAndSetsStatusToPending()
        {
            var guid = Guid.NewGuid();
            _eventRepositoryMock.Get(guid)
                .Returns(new Event()
                {
                    EventId = guid,
                    EventState = EventState.Failed
                });

            await _sut.RescheduleEventForPending(guid);

            await _eventRepositoryMock.Received().Get(guid);
            _stateGuardServiceMock.Received().AssertCanSetOrThrow(guid, EventState.Failed, EventState.Pending);
            await _eventRepositoryMock.Received().Update(Arg.Is<Event>(
                y => y.EventId == guid && y.EventState == EventState.Pending
            ));
        }

        [Fact]
        public async Task ThrowsOnError()
        {
            _eventRepositoryMock
                .When(x =>
                    x.Get(Arg.Any<Guid>()))
                .Do(x => throw new Exception());

            await Assert.ThrowsAsync<Exception>(async () => await _sut.RescheduleEventForPending(Guid.NewGuid()));
        }
    }
}