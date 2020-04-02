using DevWeek.Architecture.Extensions;
using Oragon.Spring.Objects.Factory.Attributes;
using System;

namespace DevWeek.Architecture.MessageQueuing
{
	public class ConsumerCountManager : IConsumerCountManager
	{
		[Required]
		public uint MinConcurrentConsumers { get; set; }

		[Required]
		public int MaxConcurrentConsumers { get; set; }

		[Required]
		public TimeSpan AutoscaleFrequency { get; set; }

		[Required]
		public uint MessagesPerConsumerWorkerRatio { get; set; }

		public virtual int GetScalingAmount(QueueInfo queueInfo, int consumersRunningCount)
		{
			uint consumersByRatio = queueInfo.MessageCount / MessagesPerConsumerWorkerRatio;

			int idealConsumerCount;

			if (consumersByRatio < MinConcurrentConsumers)
			{
				idealConsumerCount = MinConcurrentConsumers.ToInt();
			}
			else if (consumersByRatio > MaxConcurrentConsumers)
			{
				idealConsumerCount = MaxConcurrentConsumers.ToInt();
			}
			else
			{
				idealConsumerCount = consumersByRatio.ToInt();
			}

			int scalingAmount = idealConsumerCount - consumersRunningCount;

			return scalingAmount;
		}
	}
}