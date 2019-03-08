using Oragon.Spring.Objects.Factory.Attributes;
using System;

namespace DevWeek.Architecture.MessageQueuing
{
	public class MessageDeserializationRejectionHandler : IMessageRejectionHandler
	{
		[Required]
		public IQueueClient RabbitMQClient { get; set; }

		[Required]
		public string ExchangeName { get; set; }

		[Required]
		public string RejectionRoutingKey { get; set; }

		private const string DefaultRejectionQueueName = "RejectedMessages";

		public void OnRejection(RejectionException exception)
		{
			//Cast DeserializationException
			var deserializationException = (DeserializationException)exception;

			//Parse exception to DeserializationRejectMessage
			var message = new DeserializationRejectionMessage()
			{
				Date = DateTime.Now,
				QueueName = deserializationException.QueueName,
				SerializedDataBinary = deserializationException.SerializedDataBinary,
				SerializedDataString = deserializationException.SerializedDataString,
				SerializedException = Newtonsoft.Json.JsonConvert.SerializeObject(deserializationException)
			};

			//Ensure reject queue and binding is created
			this.EnsureQueueAndBinding();

			//Publish rejection message
			this.RabbitMQClient.Publish(this.ExchangeName, this.RejectionRoutingKey, message);
		}

		private void EnsureQueueAndBinding()
		{
			//Ensure queue is created
			this.RabbitMQClient.QueueDeclare(DefaultRejectionQueueName);

			//Ensure exchange is created
			this.RabbitMQClient.ExchangeDeclare(this.ExchangeName);

			//Bind Exchange to queue
			this.RabbitMQClient.QueueBind(DefaultRejectionQueueName, this.ExchangeName, this.RejectionRoutingKey);
		}
	}
}