using System;
using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Services;
using Moq;
using OrchardCore.Environment.Shell.Configuration;
using Xunit;
using YesSql;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Data;

public class EventTableManagerTests
{
    private readonly Mock<ISession> _sessionMock;
    private readonly Mock<IStore> _storeMock;
    private readonly Mock<IEventTableNameService> _eventTableNameServiceMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;

    private readonly EventTableManager _sut;

    public EventTableManagerTests()
    {
        _sessionMock = new Mock<ISession>();
        _storeMock = new Mock<IStore>();
        _eventTableNameServiceMock = new Mock<IEventTableNameService>();
        _serviceProviderMock = new Mock<IServiceProvider>();

        _sessionMock.SetupGet(m => m.Store).Returns(_storeMock.Object);
        _eventTableNameServiceMock.Setup(m => m.CreateTableNameWithPrefixOrWithout())
            .Returns("prefix_EventTable");

        _sut = new EventTableManager(_sessionMock.Object, _eventTableNameServiceMock.Object, _serviceProviderMock.Object);
    }

    public class CreateTableIfNotExist : EventTableManagerTests
    {
        [Fact]
        public async Task ShouldCreateTableWithPrefix()
        {
            await _sut.CreateTableIfNotExist();
            
            _storeMock.Verify(x => x.InitializeCollectionAsync("prefix_EventTable"));
        }
        
        [Fact]
        public async Task ShouldCreateTableWithoutPrefix()
        {
            _eventTableNameServiceMock.Setup(m => m.CreateTableNameWithPrefixOrWithout())
                .Returns("EventTable");

            await _sut.CreateTableIfNotExist();
            
            _storeMock.Verify(x => x.InitializeCollectionAsync("EventTable"));
        }
    }
}