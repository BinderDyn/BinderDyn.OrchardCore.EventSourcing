using System.Threading.Tasks;
using BinderDyn.OrchardCore.EventSourcing.Data;
using Moq;
using OrchardCore.Environment.Shell.Configuration;
using Xunit;
using YesSql;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Data;

public class EventTableManagerTests
{
    private readonly Mock<ISession> _sessionMock;
    private readonly Mock<IStore> _storeMock;
    private readonly Mock<IShellConfiguration> _shellConfigurationMock;

    private readonly EventTableManager _sut;

    public EventTableManagerTests()
    {
        _sessionMock = new Mock<ISession>();
        _storeMock = new Mock<IStore>();
        _shellConfigurationMock = new Mock<IShellConfiguration>();

        _sessionMock.SetupGet(m => m.Store).Returns(_storeMock.Object);
        _shellConfigurationMock.SetupGet(m => m["TablePrefix"])
            .Returns("prefix");

        _sut = new EventTableManager(_sessionMock.Object, _shellConfigurationMock.Object);
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
            _shellConfigurationMock.SetupGet(m => m["TablePrefix"])
                .Returns(string.Empty);

            await _sut.CreateTableIfNotExist();
            
            _storeMock.Verify(x => x.InitializeCollectionAsync("EventTable"));
        }
    }
}