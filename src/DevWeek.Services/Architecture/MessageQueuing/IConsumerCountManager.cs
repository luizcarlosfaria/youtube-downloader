using System;

namespace DevWeek.Architecture.MessageQueuing
{
    public interface IConsumerCountManager
    {
        int MaxConcurrentConsumers { get; }

        int GetScalingAmount(QueueInfo queueInfo, int consumersRunningCount);

        TimeSpan AutoscaleFrequency { get; }
    }
}