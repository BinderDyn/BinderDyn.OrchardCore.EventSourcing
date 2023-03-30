using System;
using BinderDyn.OrchardCore.EventSourcing.Services;
using FluentAssertions;
using Moq;
using OrchardCore.Environment.Shell.Configuration;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Services;

public class EventTableNameServiceTests
{
    private readonly Mock<IShellConfiguration> _shellConfigurationMock;
    
    private readonly EventTableNameService _sut;

    public EventTableNameServiceTests()
    {
        _shellConfigurationMock = new Mock<IShellConfiguration>();
        
        _sut = new EventTableNameService(_shellConfigurationMock.Object);
    }

    public class CreateTableNameWithPrefixOrWithout : EventTableNameServiceTests
    {
        [Fact]
        public void ShouldReturnWithPrefixIfInConfiguration()
        {
            _shellConfigurationMock.Setup(m => m["TablePrefix"])
                .Returns("Prefix");

            var result = _sut.CreateTableNameWithPrefixOrWithout();

            result.Should().Be("Prefix_EventTable");
        }
        
        [Fact]
        public void ReturnsWithoutPrefixIfNotInConfiguration()
        {
            _shellConfigurationMock.Setup(m => m["TablePrefix"])
                .Returns(string.Empty);

            var result = _sut.CreateTableNameWithPrefixOrWithout();

            result.Should().Be("EventTable");
        }
    }
}