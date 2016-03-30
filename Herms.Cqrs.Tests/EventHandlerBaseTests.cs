using System;
using Herms.Cqrs.TestContext.EventHandlers;
using Herms.Cqrs.TestContext.Events;
using Xunit;

namespace Herms.Cqrs.Tests
{
    public class EventHandlerBaseTests
    {
        [Fact]
        public void GivenExtensionOfEventHandlerBaseIsMarkedGenericHandlerWithArgument_WhenCanHandleIsCalled_ThenCanHandleShouldAnswerCorrectlyBasedOnReflection()
        {
            var testEventHandler1 = new TestEventHandler1();
            var testEventHandler2 = new TestEventHandler2();
            var testEventHandler1Instance2 = new TestEventHandler1();
            var testEventHandler2Instance2 = new TestEventHandler2();
            Assert.True(testEventHandler1.CanHandle(new TestEvent1()));
            Assert.True(testEventHandler1.CanHandle(new TestEvent2()));
            Assert.False(testEventHandler1.CanHandle(new TestEvent3()));
            Assert.False(testEventHandler2.CanHandle(new TestEvent1()));
            Assert.True(testEventHandler2.CanHandle(new TestEvent2()));
            Assert.True(testEventHandler2.CanHandle(new TestEvent3()));
            Assert.False(testEventHandler2Instance2.CanHandle(new TestEvent1()));
            Assert.True(testEventHandler2Instance2.CanHandle(new TestEvent2()));
            Assert.True(testEventHandler2Instance2.CanHandle(new TestEvent3()));
            Assert.True(testEventHandler1Instance2.CanHandle(new TestEvent1()));
            Assert.True(testEventHandler1Instance2.CanHandle(new TestEvent2()));
            Assert.False(testEventHandler1Instance2.CanHandle(new TestEvent3()));
        }
    }
}