using System;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Aggregate.Exceptions;
using Herms.Cqrs.TestContext.Models;
using Xunit;

namespace Herms.Cqrs.Tests.Aggregate
{
    public class AggregateTest
    {
        [Fact]
        public void GivenAggregate_WhenCreateModelException_ThenItShouldContainIdAndType()
        {
            var testAggregate = new TestAggregate();
            var exception = testAggregate.CreateModelException("Something went wrong.");
            Assert.Equal(typeof(TestAggregate), exception.AggregateType);
            Assert.Equal(testAggregate.Id, exception.AggregateId);
        }

        [Fact]
        public void GivenAggregate_WhenCallingThrowModelException_ThenModelExceptionShouldBeThrown()
        {
            var testAggregate = new TestAggregate();
            Assert.Throws<AggregateModelException>(() => testAggregate.ThrowModelException("Something went wrong."));
        }
    }
}