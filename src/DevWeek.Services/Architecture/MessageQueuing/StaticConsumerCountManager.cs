using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Architecture.MessageQueuing
{
    public class StaticConsumerCountManager : IConsumerCountManager
    {
        public int MaxConcurrentConsumers { get; set; }

        public TimeSpan AutoscaleFrequency { get; set; } = TimeSpan.FromMinutes(1);

        public int GetScalingAmount(QueueInfo queueInfo, int consumersRunningCount) => this.MaxConcurrentConsumers - consumersRunningCount;
    }
}
