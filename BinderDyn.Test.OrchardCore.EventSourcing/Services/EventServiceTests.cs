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
    private readonly DateTime _now = new DateTime(2023, 1, 1, 0,0,0);

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
            var expectedEvent = new Event<string>()
            {
                Created = _now,
                Payload = "SomePayload",
                PayloadType = "System.String",
                EventId = Guid.Parse("86B74411-B3A8-4D01-B253-0F1538E0AAA3"),
                Processed = null,
                OriginalEventId = null,
                EventState = EventState.Pending
            };
            string payload = "SomePayload";

            var guid = await _sut.Add(payload, "friendlyName");

            _eventRepositoryMock.Verify(x => x.Add(It.Is<Event<string>>(y => 
                y.Created == expectedEvent.Created &&
                y.Payload == expectedEvent.Payload &&
                y.EventId == expectedEvent.EventId &&
                y.Processed == expectedEvent.Processed &&
                y.PayloadType == expectedEvent.PayloadType &&
                y.EventTypeFriendlyName == "friendlyName" &&
                y.OriginalEventId == expectedEvent.OriginalEventId &&
                y.EventState == expectedEvent.EventState)));
            guid.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldThrowIfNoEventProvided()
        {
            await Assert
                .ThrowsAsync<NoEventDataProvidedException>(
                    async () => await _sut.Add<string>(null, null));
        }
    }

    public class GetNextPending : EventServiceTests
    {
        [Fact]
        public async Task ShouldReturnNullIfNoEvent()
        {
            var result = await _sut.GetNextPending<string>();

            result.Should().BeNull();
        }
        [Fact]
        public async Task ShouldReturnEventIfAvailable()
        {
            _eventRepositoryMock.Setup(m => m.GetNextPending<string>(null))
                .ReturnsAsync(new Event<string>() {Payload = "event"});
            
            var result = await _sut.GetNextPending<string>();

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
                await _sut.SetInProcessing<string>(Guid.NewGuid()));
        }
        
        [Fact]
        public async Task SetsEventInProcessing()
        {
            _eventRepositoryMock.Setup(m => m.Get<string>(It.IsAny<Guid>()))
                .ReturnsAsync(new Event<string>()
                {
                    Payload = "somePayload",
                    EventState = EventState.Pending
                });
            
            await _sut.SetInProcessing<string>(Guid.NewGuid());
            
            _eventRepositoryMock.Verify(x => 
                x.Update(It.Is<Event<string>>(y => y.EventState == EventState.InProcessing)));
        }
    }

    public class SetAsProcessed : EventServiceTests
    {
        [Fact]
        public async Task ThrowsIfNoEventFoundForId()
        {
            await Assert.ThrowsAsync<EventNotFoundException>(async () => 
                await _sut.SetAsProcessed<string>(Guid.NewGuid()));
        }
        
        [Fact]
        public async Task SetsEventAsProcessedForPending()
        {
            _eventRepositoryMock.Setup(m => m.Get<string>(It.IsAny<Guid>()))
                .ReturnsAsync(new Event<string>()
                {
                    Payload = "somePayload",
                    EventState = EventState.Pending
                });
            
            await _sut.SetAsProcessed<string>(Guid.NewGuid());
            
            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event<string>>(y => 
                y.Processed == _now &&
                y.EventState == EventState.Processed)));
        }
        
        [Fact]
        public async Task SetsEventAsProcessedForInProcessing()
        {
            _eventRepositoryMock.Setup(m => m.Get<string>(It.IsAny<Guid>()))
                .ReturnsAsync(new Event<string>()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });
            
            await _sut.SetAsProcessed<string>(Guid.NewGuid());
            
            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event<string>>(y => 
                y.Processed == _now &&
                y.EventState == EventState.Processed)));
        }
    }

    public class SetAsFailed : EventServiceTests
    {
        [Fact]
        public async Task SetsEventAsFailed()
        {
            _eventRepositoryMock.Setup(m => m.Get<string>(It.IsAny<Guid>()))
                .ReturnsAsync(new Event<string>()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });
            
            await _sut.SetAsFailed<string>(Guid.NewGuid());
            
            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event<string>>(y => 
                y.EventState == EventState.Failed)));
        }
        
        [Fact]
        public async Task ThrowsIfNoEventFoundForId()
        {
            await Assert.ThrowsAsync<EventNotFoundException>(async () => 
                await _sut.SetAsFailed<string>(Guid.NewGuid()));
        }
    }

    public class SetAsAborted : EventServiceTests
    {
        [Fact]
        public async Task SetsEventAsAborted()
        {
            _eventRepositoryMock.Setup(m => m.Get<string>(It.IsAny<Guid>()))
                .ReturnsAsync(new Event<string>()
                {
                    Payload = "somePayload",
                    EventState = EventState.InProcessing
                });
            
            await _sut.SetAsAborted<string>(Guid.NewGuid());
            
            _eventRepositoryMock.Verify(x => x.Update(It.Is<Event<string>>(y => 
                y.EventState == EventState.Aborted)));
        }
    }
}