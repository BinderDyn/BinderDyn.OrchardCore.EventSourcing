using System;
using BinderDyn.OrchardCore.EventSourcing.Enums;
using BinderDyn.OrchardCore.EventSourcing.Exceptions;
using BinderDyn.OrchardCore.EventSourcing.Services;
using Xunit;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Services;

public class StateGuardServiceTests
{
    private readonly StateGuardService _sut;

    public StateGuardServiceTests()
    {
        _sut = new StateGuardService();
    }

    public class AssertCanSetOrThrow : StateGuardServiceTests
    {
        [Theory]
        [InlineData(EventState.Aborted, EventState.Processed, typeof(EventAlreadyAbortedException))]
        [InlineData(EventState.Failed, EventState.Processed, typeof(EventAlreadyFailedException))]
        [InlineData(EventState.Aborted, EventState.InProcessing, typeof(EventAlreadyAbortedException))]
        [InlineData(EventState.Processed, EventState.InProcessing, typeof(EventAlreadyProcessedException))]
        [InlineData(EventState.Aborted, EventState.Failed, typeof(EventAlreadyAbortedException))]
        [InlineData(EventState.Processed, EventState.Failed, typeof(EventAlreadyProcessedException))]
        [InlineData(EventState.Aborted, EventState.Pending, typeof(EventAlreadyAbortedException))]
        [InlineData(EventState.Processed, EventState.Pending, typeof(EventAlreadyProcessedException))]
        [InlineData(EventState.Failed, EventState.Aborted, typeof(EventAlreadyFailedException))]
        [InlineData(EventState.Processed, EventState.Aborted, typeof(EventAlreadyProcessedException))]
        public void ThrowsCorrectException(EventState state, EventState targetState, Type exception)
        {
            Assert.Throws(exception, () => _sut.AssertCanSetOrThrow(Guid.NewGuid(), state, targetState));
        }
    }
}