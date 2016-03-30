using System;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Models;
using Xunit;

namespace Herms.Cqrs.File.Tests
{
    public class FileSystemEventRepositoryTests
    {
        [Fact]
        public void GivenEventSerialized_WhenDeserialize_ThenItShouldBeDeserializedToCorrectType()
        {
            var fileSystemEventRepo = new FileSystemEventRepository<TestAggregate>();
            var testAggregate = new TestAggregate();
            testAggregate.InvokeCommand1(new TestCommand1());
            testAggregate.InvokeCommand2(new TestCommand2());
            Assert.Equal(2, testAggregate.Version);

            fileSystemEventRepo.Save(testAggregate);

            var deserialized = fileSystemEventRepo.Get(testAggregate.Id);

            Assert.Equal(2, deserialized.Version);
        }
    }
}