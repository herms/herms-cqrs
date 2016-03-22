using System;
using Herms.Cqrs.TestContext.EventHandlers;
using Herms.Cqrs.TestContext.Events;
using Xunit;

namespace Herms.Cqrs.Tests
{
    public class EventHandlerBaseTests
    {
        [Fact]
        public void GivenEventHandlerImplementationIsMarkedWithGenericHandler_WhenCanHandleIsCalled_ThenWhat()
        {
            var testEventHandler1 = new TestEventHandler1();
            Assert.True(testEventHandler1.CanHandle(new TestEvent1()));
            Assert.True(testEventHandler1.CanHandle(new TestEvent2()));
            Assert.False(testEventHandler1.CanHandle(new TestEvent3()));
        }
    }
}