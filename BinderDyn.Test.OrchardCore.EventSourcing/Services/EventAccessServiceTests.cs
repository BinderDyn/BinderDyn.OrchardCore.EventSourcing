using System;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Services;

public class EventAccessServiceTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<ILogger<EventAccessService>> _loggerMock;
    private readonly Mock<IStateGuardService> _stateGuardServiceMock;
    private readonly EventAccessService _sut;

    public EventAccessServiceTests()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _loggerMock = new Mock<ILogger<EventAccessService>>();
        _stateGuardServiceMock = new Mock<IStateGuardService>();

        _sut = new EventAccessService(_eventRepositoryMock.Object, _loggerMock.Object, _stateGuardServiceMock.Object);
    }

    public class GetAllFiltered : EventAccessServiceTests
    {
        [Fact]
        public async Task ShouldReturnViewModelsFromEventsFetched()
        {
            _eventRepositoryMock
                .Setup(m => m.GetPagedByStates(0, 5, It.IsAny<EventState[]>()))
                .ReturnsAsync(new[]
                {
                    new Event(),
                    new Event(),
                    new Event()
                });

            var result = await _sut.GetAllFiltered(new EventFilter()
            {
                Skip = 0,
                Take = 5,
                States = new[] {EventState.Pending}
            });

            _eventRepositoryMock.Verify(x => x.GetPagedByStates(0, 5, new[] {EventState.Pending}));

            result.Length.Should().Be(3);
        }

        [Fact]
        public async Task ReturnEmptyListIfNothingFoundForCriteria()
        {
            var result = await _sut.GetAllFiltered(new EventFilter()
            {
                Skip = 0,
                Take = 5,
                States = new[] {EventState.Pending}
            });

            _eventRepositoryMock.Verify(x => x.GetPagedByStates(0, 5, new[] {EventState.Pending}));

            result.Length.Should().Be(0);
        }

        [Fact]
        public async Task ThrowsOnError()
        {
            _eventRepositoryMock.Setup(m =>
                    m.GetPagedByStates(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<EventState[]>()))
                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(async () => await _sut.GetAllFiltered(new EventFilter()));
        }
    }

    public class RescheduleEventForPending : EventAccessServiceTests
    {
        [Fact]
        public async Task FetchesEventAndSetsStatusToPending()
        {
            var guid = Guid.NewGuid();
            _eventRepositoryMock.Setup(m => m.Get(guid))
                .ReturnsAsync(new Event()
                {
                    EventId = guid,
                    EventState = EventState.Failed
                });

            await _sut.RescheduleEventForPending(guid);

            _eventRepositoryMock.Verify(x => x.Get(guid));
            _stateGuardServiceMock.Verify(x => x.AssertCanSetOrThrow(guid, EventState.Failed, EventState.Pending));
            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event>(
                y => y.EventId == guid && y.EventState == EventState.Pending
            )));
        }

        [Fact]
        public async Task ThrowsOnError()
        {
            _eventRepositoryMock.Setup(m => m.Get(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(async () => await _sut.RescheduleEventForPending(Guid.NewGuid()));
        }
    }
}