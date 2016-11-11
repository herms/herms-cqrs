using System;
using Xunit;

namespace Herms.Cqrs.Azure.Tests
{
    public class QueueWaitStateTest
    {
        [Fact]
        public void GivenQueueWaitState_WhenGetWait_ThenWaitShouldIncreaseUntilMaxIsReached()
        {
            var queueWaitState = new QueueWaitState(100, 500, 2);

            Assert.Equal(100, queueWaitState.GetWait());
            Assert.Equal(200, queueWaitState.GetWait());
            Assert.Equal(400, queueWaitState.GetWait());
            Assert.Equal(500, queueWaitState.GetWait());
            Assert.Equal(500, queueWaitState.GetWait());
            Assert.Equal(500, queueWaitState.GetWait());
            queueWaitState.Reset();
            Assert.Equal(100, queueWaitState.GetWait());
            Assert.Equal(200, queueWaitState.GetWait());
            Assert.Equal(400, queueWaitState.GetWait());
            Assert.Equal(500, queueWaitState.GetWait());
            Assert.Equal(500, queueWaitState.GetWait());
            Assert.Equal(500, queueWaitState.GetWait());
        }
    }
}