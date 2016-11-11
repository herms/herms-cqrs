using System;

namespace Herms.Cqrs.Azure
{
    public class AzureStorageQueueConfiguration
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
        public int MinWaitMs { get; set; } = 100;
        public double WaitMultiplier { get; set; } = 1.5;
        public int MaxWaitMs { get; set; } = 5000;
    }

    public class QueueWaitState
    {
        private readonly int _maxWaitMs;
        private readonly int _minWaitMs;
        private readonly double _waitMultipler;
        private int _currentWait;

        public QueueWaitState(int minWaitMs, int maxWaitMs, double waitMultipler)
        {
            _minWaitMs = minWaitMs;
            _maxWaitMs = maxWaitMs;
            _waitMultipler = waitMultipler;
            _currentWait = minWaitMs;
        }

        public int GetWait()
        {
            var returnValue = _currentWait;
            _currentWait = (int) (_currentWait * _waitMultipler);
            if (_currentWait >= _maxWaitMs)
                _currentWait = _maxWaitMs;
            return returnValue;
        }

        public void Reset()
        {
            _currentWait = _minWaitMs;
        }
    }
}