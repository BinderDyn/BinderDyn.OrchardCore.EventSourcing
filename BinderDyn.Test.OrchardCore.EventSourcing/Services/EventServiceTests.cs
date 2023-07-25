using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using BinderDyn.OrchardCore.EventSourcing.Wrapper;
using FluentAssertions;
using Moq;
using OrchardCore.Modules;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Services;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<IStateGuardService> _stateGuardServiceMock;
    private readonly DateTime _now = new(2023, 1, 1, 0, 0, 0);

    private readonly EventService _sut;

    protected EventServiceTests()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        var clockMock = new Mock<IClock>();
        var guidWrapperMock = new Mock<IGuidWrapper>();
        _stateGuardServiceMock = new Mock<IStateGuardService>();

        guidWrapperMock.SetupSequence(m => m.NewGuid())
            .Returns(Guid.Parse("86B74411-B3A8-4D01-B253-0F1538E0AAA3"))
            .Returns(Guid.Parse("C3C849FE-2EE5-4197-A683-B10F34F095B9"));

        clockMock.Setup(m => m.UtcNow).Returns(_now);

        _sut = new EventService(
            _eventRepositoryMock.Object,
            clockMock.Object,
            guidWrapperMock.Object,
            _stateGuardServiceMock.Object);
    }

    public class Add : EventServiceTests
    {
        [Fact]
        public async Task AddsAnEventToTheEventRepository()
        {
            var payload = "SomePayload";

            var guid = await _sut.Add(payload, "friendlyName");

            guid.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldThrowIfNoEventProvided()
        {
            await Assert
                .ThrowsAsync<NoEventDataProvidedException>(
                    async () => await _sut.Add(null, null));
        }
    }

    public class GetNextPending : EventServiceTests
    {
        [Fact]
        public async Task ShouldReturnNullIfNoEvent()
        {
            var result = await _sut.GetNextPending();

            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldReturnEventIfAvailable()
        {
            _eventRepositoryMock.Setup(m => m.GetNextPending(null))
                .ReturnsAsync(new Event() {Payload = "event"});

            var result = await _sut.GetNextPending();

            result.Should().NotBeNull();
            result.Payload.Should().Be("event");
        }
    }

    public class SetInProcessing : EventServiceTests
    {
        [Fact]
        public async Task ThrowsIfNoEventFoundForId()
        {
            await Assert.ThrowsAsync<EventNotFoundException>(async () =>
                await _sut.SetInProcessing(Guid.NewGuid()));
        }

        [Fact]
        public async Task SetsEventInProcessing()
        {
            _eventRepositoryMock.Setup(m => m.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.Pending
                });

            await _sut.SetInProcessing(Guid.NewGuid());

            _eventRepositoryMock.Verify(x =>
                x.Update(It.Is<Event>(y => y.EventState == EventState.InProcessing)));
        }
    }

    public class SetAsProcessed : EventServiceTests
    {
        [Fact]
        public async Task ThrowsIfNoEventFoundForId()
        {
            await Assert.ThrowsAsync<EventNotFoundException>(async () =>
                await _sut.SetAsProcessed(Guid.NewGuid()));
        }

        [Fact]
        public async Task SetsEventAsProcessedForPending()
        {
            _eventRepositoryMock.Setup(m => m.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.Pending
                });

            await _sut.SetAsProcessed(Guid.NewGuid());

            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event>(y =>
                y.ProcessedUtc == _now &&
                y.EventState == EventState.Processed)));
        }

        [Fact]
        public async Task SetsEventAsProcessedForInProcessing()
        {
            _eventRepositoryMock.Setup(m => m.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });

            await _sut.SetAsProcessed(Guid.NewGuid());

            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event>(y =>
                y.ProcessedUtc == _now &&
                y.EventState == EventState.Processed)));
        }
    }

    public class SetAsFailed : EventServiceTests
    {
        [Fact]
        public async Task SetsEventAsFailed()
        {
            _eventRepositoryMock.Setup(m => m.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });

            await _sut.SetAsFailed(Guid.NewGuid());

            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event>(y =>
                y.EventState == EventState.Failed)));
        }

        [Fact]
        public async Task ThrowsIfNoEventFoundForId()
        {
            await Assert.ThrowsAsync<EventNotFoundException>(async () =>
                await _sut.SetAsFailed(Guid.NewGuid()));
        }
    }

    public class SetAsAborted : EventServiceTests
    {
        [Fact]
        public async Task SetsEventAsAborted()
        {
            _eventRepositoryMock.Setup(m => m.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });

            await _sut.SetAsAborted(Guid.NewGuid());

            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event>(y =>
                y.EventState == EventState.Aborted)));
        }
    }
}