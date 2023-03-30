using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Models;
using BinderDyn.OrchardCore.EventSourcing.Services;
using Moq;
using Xunit;
using YesSql;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Data;

public class EventRepositoryTests
{
    private readonly Mock<ISession> _sessionMock;
    private readonly Mock<IEventTableNameService> _eventTableNameServiceMock;
    private readonly EventRepository _sut;

    public EventRepositoryTests()
    {
        _sessionMock = new Mock<ISession>();
        _eventTableNameServiceMock = new Mock<IEventTableNameService>();

        _eventTableNameServiceMock.Setup(m => m.CreateTableNameWithPrefixOrWithout())
            .Returns("EventTable");

        _sut = new EventRepository(_sessionMock.Object, _eventTableNameServiceMock.Object);
    }

    public class Add : EventRepositoryTests
    {
        [Fact]
        public async Task ShouldAddEvent()
        {
            await _sut.Add(new Event<string>());
            
            _sessionMock.Verify(x => x.Save(It.IsAny<Event<string>>(), false, "EventTable"));
            _sessionMock.Verify(x => x.SaveChangesAsync());
        }
        
        [Fact]
        public async Task ShouldDoNothingIfNull()
        {
            await _sut.Add((Event<string>)null);
            
            _sessionMock.VerifyNoOtherCalls();
        }
    }
}