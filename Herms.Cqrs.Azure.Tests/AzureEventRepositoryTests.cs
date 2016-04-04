using System;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Models;
using Xunit;

namespace Herms.Cqrs.Azure.Tests
{
    public class AzureEventRepositoryTests
    {
        private readonly AzureEventRepository<TestAggregate> _sut;

        public AzureEventRepositoryTests()
        {
            _sut = new AzureEventRepository<TestAggregate>(true);
        }

        [Fact]
        public void GivenAggregateWithChanges_WhenSaving_ThenChangesShouldBePersisted()
        {
            var aggregate = new TestAggregate();
            aggregate.InvokeCommand1(new TestCommand1());
            aggregate.InvokeCommand2(new TestCommand2());

            _sut.Save(aggregate);

            var loadedAggregate = _sut.Get(aggregate.Id);

            Assert.NotNull(loadedAggregate);
            Assert.Equal(aggregate.Id, loadedAggregate.Id);
            Assert.Equal(aggregate.Version, loadedAggregate.Version);
        }

        [Fact]
        public void GivenTwoAggregatesWithChanges_WhenSavingAndLoading_ThenTheEventsDontGetMixed()
        {
            var a1 = new TestAggregate();
            var a2 = new TestAggregate();

            a1.InvokeCommand1(new TestCommand1());
            a1.InvokeCommand2(new TestCommand2 {Param1 = "A1"});
            a2.InvokeCommand1(new TestCommand1());
            a2.InvokeCommand2(new TestCommand2 {Param1 = "A2"});

            _sut.Save(a1);
            _sut.Save(a2);

            a1 = _sut.Get(a1.Id);
            a2 = _sut.Get(a2.Id);

            Assert.Equal("A1", a1.Prop1);
            Assert.Equal("A2", a2.Prop1);
        }
    }
}