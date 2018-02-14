using DevWeek.Architecture.MessageQueuing;

namespace DevWeek.Architecture.Workflow.QueuedWorkFlow
{
	public class AmqpBasedRoute
	{
		public string ExchangeName { get; set; }

		public string QueueName { get; set; }

		public string RoutingKey { get; set; }

		public void Bind(IQueueClient queueClient)
		{
			queueClient.QueueBind(this.QueueName, this.ExchangeName, this.RoutingKey);
		}

		public void ExchangeDeclare(IQueueClient queueClient)
		{
			queueClient.ExchangeDeclare(this.ExchangeName);
		}

		public void EnsureQueueExists(IQueueClient queueClient)
		{
			queueClient.EnsureQueueExists(this.QueueName);
		}

		public void EnsureAll(IQueueClient queueClient)
		{
			this.ExchangeDeclare(queueClient);
			this.EnsureQueueExists(queueClient);
			this.Bind(queueClient);
		}
	}
}