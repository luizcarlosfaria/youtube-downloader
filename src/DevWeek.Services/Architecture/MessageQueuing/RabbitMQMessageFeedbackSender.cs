using RabbitMQ.Client;

namespace DevWeek.Architecture.MessageQueuing;

	public class RabbitMQMessageFeedbackSender : IMessageFeedbackSender
	{
		private readonly IModel _model;

		public ulong DeliveryTag { get; private set; }

		public bool MessageAcknoledged { get; private set; }

		public RabbitMQMessageFeedbackSender(IModel model, ulong deliveryTag)
		{
			this._model = model;
			this.DeliveryTag = deliveryTag;

			this.MessageAcknoledged = false;
		}

		public void Ack()
		{
			if (!this.MessageAcknoledged)
			{
				this._model.BasicAck(this.DeliveryTag, false);

				this.MessageAcknoledged = true;
			}
		}

		public void Nack(bool requeue)
		{
			if (!this.MessageAcknoledged)
			{
				this._model.BasicNack(this.DeliveryTag, false, requeue);

				this.MessageAcknoledged = true;
			}
		}
	}