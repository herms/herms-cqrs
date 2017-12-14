using System;
using System.Threading.Tasks;
using Herms.Cqrs.Saga.Exceptions;
using Herms.Cqrs.TestContext;
using Herms.Cqrs.TestContext.Commands;
using Moq;
using Xunit;

namespace Herms.Cqrs.Tests
{
    public class SagaTest
    {
        [Fact]
        public async Task GivenSagaWithMissingCommands_WhenProceedWithSaga_ThenConsistencyCheckShouldFail()
        {
            var infrastructureRepositoryMock = new Mock<IInfrastructureRepository>();
            var commandHandlerRegistryMock = new Mock<ICommandHandlerRegistry>();
            var dummyHandler = new Mock<ICommandHandler<Command>>();

            commandHandlerRegistryMock.Setup(ch => ch.ResolveHandler(It.Is<Command>(t => t.GetType() == typeof(TestCommand1))))
                .Returns(dummyHandler.Object);
            commandHandlerRegistryMock.Setup(ch => ch.ResolveHandler(It.Is<Command>(t => t.GetType() == typeof(TestCommand2))))
                .Returns(dummyHandler.Object);
            commandHandlerRegistryMock.Setup(ch => ch.ResolveHandler(It.Is<Command>(t => t.GetType() == typeof(TestCommand3))))
                .Returns(dummyHandler.Object);

            var correlationId = Guid.NewGuid();
            var saga = new TestSaga(correlationId, commandHandlerRegistryMock.Object, infrastructureRepositoryMock.Object);
            saga.TestCommand1 = new TestCommand1 { Status = CommandStatus.Processed };
            saga.TestCommand3 = new TestCommand3();
            //Command.Correlate(saga.TestCommand1, saga.TestCommand3);

            await Assert.ThrowsAsync<SagaConsistencyException>(() => saga.ProceedAsync());
        }

        [Fact]
        public async Task GivenSagaWithFailedCommands_WhenProceedWithSaga_ThenExceptionShouldBeThrown()
        {
            var infrastructureRepositoryMock = new Mock<IInfrastructureRepository>();
            var commandHandlerRegistryMock = new Mock<ICommandHandlerRegistry>();

            var correlationId = Guid.NewGuid();
            var saga = new TestSaga(correlationId, commandHandlerRegistryMock.Object, infrastructureRepositoryMock.Object);
            saga.TestCommand1 = new TestCommand1();
            saga.TestCommand2 = new TestCommand2();
            saga.TestCommand3 = new TestCommand3();
            Command.Correlate(saga.TestCommand1, saga.TestCommand2, saga.TestCommand3);
            saga.TestCommand1.Status = CommandStatus.Failed;

            await Assert.ThrowsAsync<SagaException>(() => saga.ProceedAsync());
        }
    }
}