using System;
using System.Configuration;
using System.Threading.Tasks;
using Herms.Cqrs.Registration;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Events;
using Herms.Cqrs.TestContext.Models;
using Xunit;

namespace Herms.Cqrs.Azure.Tests
{
    public class AzureEventRepositoryTests
    {
        private readonly AzureEventRepository<TestAggregate> _sut;

        public AzureEventRepositoryTests()
        {
            var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var eventMappingRegistry = new EventMappingRegistry();
            eventMappingRegistry.Register(new EventMapping {EventName = typeof(TestEvent1).FullName, EventType = typeof(TestEvent1)});
            eventMappingRegistry.Register(new EventMapping {EventName = typeof(TestEvent2).FullName, EventType = typeof(TestEvent2)});
            eventMappingRegistry.Register(new EventMapping {EventName = typeof(TestEvent3).FullName, EventType = typeof(TestEvent3)});
            _sut = new AzureEventRepository<TestAggregate>(connectionString, typeof(TestAggregate).Name, true, eventMappingRegistry);
        }

        [Fact]
        public async Task GivenAggregateWithChanges_WhenSaving_ThenChangesShouldBePersisted()
        {
            var aggregate = new TestAggregate();
            aggregate.InvokeCommand1(new TestCommand1());
            aggregate.InvokeCommand2(new TestCommand2());

            await _sut.SaveAsync(aggregate);

            await Task.Delay(50);

            var loadedAggregate = _sut.Get(aggregate.Id);

            Assert.NotNull(loadedAggregate);
            Assert.Equal(aggregate.Id, loadedAggregate.Id);
            Assert.Equal(aggregate.Version, loadedAggregate.Version);
        }

        [Fact]
        public async Task GivenTwoAggregatesWithChanges_WhenSavingAndLoading_ThenTheEventsDontGetMixed()
        {
            var a1 = new TestAggregate();
            var a2 = new TestAggregate();

            a1.InvokeCommand1(new TestCommand1());
            a1.InvokeCommand2(new TestCommand2 { Param1 = "A1-1" });
            a2.InvokeCommand1(new TestCommand1());
            a1.InvokeCommand2(new TestCommand2 { Param1 = "A1-2" });
            a2.InvokeCommand2(new TestCommand2 { Param1 = "A2-1" });

            var t1 = _sut.SaveAsync(a1);
            var t2 = _sut.SaveAsync(a2);
            Task.WaitAll(new[] { t1, t2 });
            
            await Task.Delay(50);

            a1 = _sut.Get(a1.Id);
            a2 = _sut.Get(a2.Id);

            Assert.Equal("A1-2", a1.Prop1);
            Assert.Equal("A2-1", a2.Prop1);

            Assert.True(a1.Prop1History.Contains("A1-1"));
        }
    }
}