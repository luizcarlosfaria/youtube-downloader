using System;
using System.Collections.Generic;

namespace DevWeek.Architecture.MessageQueuing
{
	public interface IQueueClient : IDisposable
	{
		void Publish<T>(string exchangeName, string routingKey, T content);

		void BatchPublish<T>(string exchangeName, string routingKey, IEnumerable<T> contentList);

		void BatchPublishTransactional<T>(string exchangeName, string routingKey, IEnumerable<T> contentList);

		IQueueConsumer GetConsumer(string queueName, IConsumerCountManager consumerCountManager, IMessageProcessingWorker messageProcessingWorker, Type expectedType, IMessageRejectionHandler messageRejectionHandler);

		void QueueDeclare(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null);

		void QueueDeclarePassive(string queueName);

		void QueueBind(string queueName, string exchangeName, string routingKey);

		uint QueuePurge(string queueName);

		void ExchangeDeclare(string exchangeName, bool passive = false);

		bool QueueExists(string queueName);

		void EnsureQueueExists(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null);

		uint GetMessageCount(string queueName);

		uint GetConsumerCount(string queueName);
	}
}