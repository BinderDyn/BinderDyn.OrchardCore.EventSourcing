using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Enums;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;
using BinderDyn.OrchardCore.EventSourcing.SqlServer.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Data;

public class EventRepositoryTests
{
    private readonly EventSourcingSqlDbContext _dbContext;
    private readonly EventRepository _sut;

    public EventRepositoryTests()
    {
        _dbContext = new EventSourcingSqlDbContext(Substitute.For<IServiceProvider>(), builder =>
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        _sut = new EventRepository(_dbContext);
    }

    public class Add : EventRepositoryTests
    {
        [Fact]
        public async Task ShouldAddEvent()
        {
            await _sut.Add(new Event.EventCreationParam()
            {
                ReferenceId = "1",
                Payload = "1",
                PayloadType = "string",
                EventTypeFriendlyName = "TestEvent"
            });

            (await _dbContext.Events.CountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task ShouldThrowIfNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Add(null));
        }
    }

    public class Update : EventRepositoryTests
    {
        [Fact]
        public async Task ShouldUpdateEvent()
        {
            var existingEvent = new Event()
            {
                Payload = "something"
            };
            _dbContext.Add(existingEvent);
            await _dbContext.SaveChangesAsync();

            await _sut.Update(new Event() { EventId = existingEvent.EventId, Payload = "somethingElse" });

            (await _dbContext.Events.FirstAsync()).Payload.Should().Be("somethingElse");
        }

        [Fact]
        public async Task ShouldThrowIfNoEventForThisId()
        {
            await Assert.ThrowsAsync<EventNotFoundException>(() =>
                _sut.Update(new Event() { EventId = Guid.NewGuid(), Payload = "irrelevant" }));
        }
    }

    public class Get : EventRepositoryTests
    {
        [Fact]
        public async Task ShouldGetEventById()
        {
            var existingEvent = new Event()
            {
                Payload = "something"
            };
            _dbContext.Add(existingEvent);
            await _dbContext.SaveChangesAsync();

            var result = await _sut.Get(existingEvent.EventId);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldThrowIfNoEventForThisId()
        {
            await Assert.ThrowsAsync<EventNotFoundException>(() =>
                _sut.Get(Guid.NewGuid()));
        }
    }

    public class GetNextPending : EventRepositoryTests
    {
        [Fact]
        public async Task ShouldGetNextPendingEventIfAvailable()
        {
            var existingEvent = new Event() { EventState = EventState.Pending };
            _dbContext.Add(existingEvent);
            await _dbContext.SaveChangesAsync();

            var result = await _sut.GetNextPending();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldReturnNullIfNoNextPending()
        {
            var result = await _sut.GetNextPending();

            result.Should().BeNull();
        }
    }

    public class GetPagedByStates : EventRepositoryTests
    {
        [Fact]
        public async Task GetsPagedByState()
        {
            var events = new List<Event>();
            for (var i = 1; i < 31; i++)
                events.Add(new Event()
                {
                    ReferenceId = i.ToString(),
                    EventState = i % 2 == 0 ? EventState.Processed : EventState.InProcessing,
                    Payload = i.ToString()
                });

            _dbContext.AddRange(events);
            await _dbContext.SaveChangesAsync();

            var result = await _sut.GetPagedByStates(5, 10, EventState.Processed, EventState.InProcessing);

            result.First().ReferenceId.Should().Be("6");
            result.First().EventState.Should().Be(EventState.Processed);
        }
    }
}