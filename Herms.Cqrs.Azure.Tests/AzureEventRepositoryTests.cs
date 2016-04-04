using System;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Models;
using Xunit;

namespace Herms.Cqrs.Azure.Tests
{
    public class AzureEventRepositoryTests
    {
        [Fact]
        public void GivenAggregateWithChanges_WhenSaving_ThenChangesShouldBePersisted()
        {
            var aggregate = new TestAggregate();
            aggregate.InvokeCommand1(new TestCommand1());
            aggregate.InvokeCommand2(new TestCommand2());

            var sut = new AzureEventRepository<TestAggregate>(true);

            sut.Save(aggregate);

            var loadedAggregate = sut.Get(aggregate.Id);

            Assert.NotNull(loadedAggregate);
            Assert.Equal(aggregate.Id, loadedAggregate.Id);
            Assert.Equal(aggregate.Version, loadedAggregate.Version);
        }
    }
}