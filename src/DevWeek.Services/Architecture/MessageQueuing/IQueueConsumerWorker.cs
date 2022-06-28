using System;

namespace DevWeek.Architecture.MessageQueuing;

	public interface IQueueConsumerWorker : IDisposable
	{
		bool ModelIsClosed { get; }

		void DoConsume();

		void Ack(ulong deliveryTag);

		void Nack(ulong deliveryTag, bool requeue = false);
	}