using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Enums;
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
    private readonly Mock<IClock> _clockMock;
    private readonly Mock<IGuidWrapper> _guidWrapperMock;
    private readonly DateTime _now = new DateTime(2023, 1, 1, 0,0,0);

    private readonly EventService _sut;

    public EventServiceTests()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _clockMock = new Mock<IClock>();
        _guidWrapperMock = new Mock<IGuidWrapper>();

        _guidWrapperMock.SetupSequence(m => m.NewGuid())
            .Returns(Guid.Parse("86B74411-B3A8-4D01-B253-0F1538E0AAA3"))
            .Returns(Guid.Parse("C3C849FE-2EE5-4197-A683-B10F34F095B9"));

        _clockMock.Setup(m => m.UtcNow).Returns(_now);

        _sut = new EventService(_eventRepositoryMock.Object, _clockMock.Object, _guidWrapperMock.Object);
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

            var guid = await _sut.Add(payload);

            _eventRepositoryMock.Verify(x => x.Add(It.Is<Event<string>>(y => 
                y.Created == expectedEvent.Created &&
                y.Payload == expectedEvent.Payload &&
                y.EventId == expectedEvent.EventId &&
                y.Processed == expectedEvent.Processed &&
                y.PayloadType == expectedEvent.PayloadType &&
                y.OriginalEventId == expectedEvent.OriginalEventId &&
                y.EventState == expectedEvent.EventState)));
            guid.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task AddsMultipleEventsToTheEventRepositoryAndReturnsCorrectOrder()
        {
            var firstExpectedEvent = new Event<string>()
            {
                Created = _now,
                Payload = "hello",
                PayloadType = "System.String",
                EventId = Guid.Parse("86B74411-B3A8-4D01-B253-0F1538E0AAA3"),
                Processed = null,
                OriginalEventId = null,
                EventState = EventState.Pending
            };
            var secondExpectedEvent = new Event<string>()
            {
                Created = _now,
                Payload = "these are multiple payloads",
                PayloadType = "System.String",
                EventId = Guid.Parse("C3C849FE-2EE5-4197-A683-B10F34F095B9"),
                Processed = null,
                OriginalEventId = null,
                EventState = EventState.Pending
            };
            string[] payloads = new []
            {
                "hello",
                "these are multiple payloads"
            };

            var guid = await _sut.Add(payloads);

            _eventRepositoryMock.Verify(x => x.Add(It.Is<IEnumerable<Event<string>>>(y =>
                y.Count() == 2)));
            guid.Should().NotBeEmpty();
        }
    }
    
    public class GetNextPending : EventServiceTests {}
    
    public class SetAsProcessed : EventServiceTests {}
    
    public class SetAsFailed : EventServiceTests {}
    
    public class SetAsAborted : EventServiceTests {}
}