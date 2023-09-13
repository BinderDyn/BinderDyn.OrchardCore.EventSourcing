using System;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Enums;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;
using BinderDyn.OrchardCore.EventSourcing.Services;
using FluentAssertions;
using NSubstitute;
using OrchardCore.Modules;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Services;

public class EventServiceTests
{
    private readonly IEventRepository _eventRepositoryMock;
    private readonly IStateGuardService _stateGuardServiceMock;
    private readonly DateTime _now = new(2023, 1, 1, 0, 0, 0);

    private readonly EventService _sut;

    protected EventServiceTests()
    {
        _eventRepositoryMock = Substitute.For<IEventRepository>();
        var clockMock = Substitute.For<IClock>();
        _stateGuardServiceMock = Substitute.For<IStateGuardService>();

        clockMock.UtcNow.Returns(_now);
        _eventRepositoryMock.Add(Arg.Any<Event.EventCreationParam>())
            .ReturnsForAnyArgs(Guid.NewGuid());

        _sut = new EventService(
            _eventRepositoryMock,
            clockMock,
            _stateGuardServiceMock);
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
            _eventRepositoryMock.GetNextPending(null)
                .ReturnsForAnyArgs(new Event() { Payload = "event" });

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
            _eventRepositoryMock.Get(Arg.Any<Guid>())
                .Returns(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.Pending
                });

            await _sut.SetInProcessing(Guid.NewGuid());

            await _eventRepositoryMock.Received().Update(Arg.Is<Event>(x => x.EventState == EventState.InProcessing));
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
            _eventRepositoryMock.Get(Arg.Any<Guid>())
                .Returns(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.Pending
                });

            await _sut.SetAsProcessed(Guid.NewGuid());

            await _eventRepositoryMock
                .Received()
                .Update(Arg.Is<Event>(x => x.ProcessedUtc == _now &&
                                           x.EventState == EventState.Processed));
        }

        [Fact]
        public async Task SetsEventAsProcessedForInProcessing()
        {
            _eventRepositoryMock.Get(Arg.Any<Guid>())
                .Returns(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });

            await _sut.SetAsProcessed(Guid.NewGuid());

            await _eventRepositoryMock
                .Received()
                .Update(Arg.Is<Event>(x => x.ProcessedUtc == _now &&
                                           x.EventState == EventState.Processed));
        }
    }

    public class SetAsFailed : EventServiceTests
    {
        [Fact]
        public async Task SetsEventAsFailed()
        {
            _eventRepositoryMock.Get(Arg.Any<Guid>())
                .ReturnsForAnyArgs(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });

            await _sut.SetAsFailed(Guid.NewGuid());

            await _eventRepositoryMock.Received().Update(Arg.Is<Event>(y =>
                y.EventState == EventState.Failed));
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
            _eventRepositoryMock.Get(Arg.Any<Guid>())
                .Returns(new Event()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });

            await _sut.SetAsAborted(Guid.NewGuid());

            await _eventRepositoryMock.Received().Update(Arg.Is<Event>(y =>
                y.EventState == EventState.Aborted));
        }
    }
}