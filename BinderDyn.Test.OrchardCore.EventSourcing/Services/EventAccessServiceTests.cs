using BinderDyn.OrchardCore.EventSourcing.Data;
using BinderDyn.OrchardCore.EventSourcing.Services;
using Moq;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Services;

public class EventAccessServiceTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly EventAccessService _sut;

    public EventAccessServiceTests()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();

        _sut = new EventAccessService(_eventRepositoryMock.Object);
    }
}