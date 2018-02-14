using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevWeek.Architecture.MessageQueuing
{
	public class RabbitMQConsumerWorker : IQueueConsumerWorker
	{
		private readonly Subscription _subscription;
		private readonly IModel _model;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly IMessageProcessingWorker _messageProcessingWorker;
		private readonly IMessageRejectionHandler _messageRejectionHandler;
		private readonly Func<int> _scaleCallbackFunc;
		private readonly Type _expectedType;

		public TimeSpan CheckAliveFrequency { get; set; }

		public Task ConsumerWorkerTask { get; private set; }

		public bool ModelIsClosed
		{
			get { return this._model.IsClosed; }
		}

		public RabbitMQConsumerWorker(IConnection connection, string queueName, IMessageProcessingWorker messageProcessingWorker, IMessageRejectionHandler messageRejectionHandler, Func<int> scaleCallbackFunc, Type expectedType, CancellationToken parentToken)
		{
			Contract.Requires(connection != null, "connection is required");
			Contract.Requires(string.IsNullOrWhiteSpace(queueName) == false, "queueName is required");
			Contract.Requires(messageProcessingWorker != null, "messageProcessingWorker is required");
			Contract.Requires(scaleCallbackFunc != null, "scaleCallbackFunc is required");

			this._model = connection.CreateModel();
			this._model.BasicQos(0, 1, false);
			this._subscription = new Subscription(this._model, queueName, false);
			this._messageProcessingWorker = messageProcessingWorker;
			this._messageRejectionHandler = messageRejectionHandler;
			this._cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(parentToken);
			this._scaleCallbackFunc = scaleCallbackFunc;
			this._expectedType = expectedType;
			this.CheckAliveFrequency = new TimeSpan(0, 0, 10);
		}

		public void DoConsume()
		{
			//Iterate while thread hasn't been canceled
			while (!this._cancellationTokenSource.Token.IsCancellationRequested)
			{
				//Create BasicDeliverEventArgs object and start trying to get the next message on the queue
				BasicDeliverEventArgs lastResult;
				bool messageReceived = this._subscription.Next(Convert.ToInt32(this.CheckAliveFrequency.TotalMilliseconds), out lastResult);

				//If a message hasn't been succesfully fetched from the queue
				if (!messageReceived)
				{
					//If the model has been closed
					if (!this._model.IsOpen)
					{
						//Dispose ConsumerWorker
						this.Dispose();

						//Throw AlreadyClosedException (model is already closed)
						throw new RabbitMQ.Client.Exceptions.AlreadyClosedException(this._model.CloseReason);
					}
				}

				//If something was in fact returned from the queue
				if (lastResult != null)
				{
					//Get message body
					string messageBody = GetBody(lastResult);

					//Create empty messageObject instance
					object messageObject = null;

					try
					{
						//Try to deserialize message body into messageObject
						messageObject =  Newtonsoft.Json.JsonConvert.DeserializeObject(messageBody, this._expectedType);
					}
					catch (Exception exception)
					{
						//Create DeserializationException to pass to RejectionHandler
						var deserializationException = new DeserializationException("Unable to deserialize data.", exception)
						{
							SerializedDataString = messageBody,
							SerializedDataBinary = lastResult.Body,
							QueueName = this._subscription.QueueName
						};
						//Pass DeserializationException to RejectionHandler
						this._messageRejectionHandler.OnRejection(deserializationException);

						//Remove message from queue after RejectionHandler dealt with it
						this.Nack(lastResult.DeliveryTag, false);
					}

					//If message has been successfully deserialized and messageObject is populated
					if (messageObject != null)
					{
						//Create messageFeedbackSender instance with corresponding model and deliveryTag
						IMessageFeedbackSender messageFeedbackSender = new RabbitMQMessageFeedbackSender(_model, lastResult.DeliveryTag);

						try
						{
							//Call given messageProcessingWorker's OnMessage method to proceed with message processing
							_messageProcessingWorker.OnMessage(messageObject, messageFeedbackSender);

							//If message has been processed with no errors but no Acknoledgement has been given
							if (!messageFeedbackSender.MessageAcknoledged)
							{
								//Acknoledge message
								this._subscription.Ack();
							}
						}
						catch (Exception)
						{
							//If something went wrong with message processing and message hasn't been acknoledged yet
							if (!messageFeedbackSender.MessageAcknoledged)
							{
								//Negatively Acknoledge message, asking for requeue
								this.Nack(lastResult.DeliveryTag, true);
							}

							//Rethrow catched Exception
							throw;
						}
					}
				}

				//In the end of the consumption loop, check if scaleDown has been requested
				if (this._scaleCallbackFunc != null && this._scaleCallbackFunc() < 0)
				{
					//If so, break consumption loop to let the thread end gracefully
					break;
				}
			}
		}

		private static string GetBody(BasicDeliverEventArgs basicDeliverEventArgs)
		{
			return Encoding.UTF8.GetString(basicDeliverEventArgs.Body);
		}

		public void Ack(ulong deliveryTag)
		{
			_model.BasicAck(deliveryTag, false);
		}

		public void Nack(ulong deliveryTag, bool requeue = false)
		{
			_model.BasicNack(deliveryTag, false, requeue);
		}

		public void Dispose()
		{
			this._subscription.Close();
			this._model.Dispose();
		}
	}
}