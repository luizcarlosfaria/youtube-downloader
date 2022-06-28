using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevWeek.Architecture.MessageQueuing;

public class RabbitMQConsumerWorker : IQueueConsumerWorker
{
    private readonly EventingBasicConsumer consumer;
    private readonly IModel model;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly IMessageProcessingWorker messageProcessingWorker;
    private readonly IMessageRejectionHandler messageRejectionHandler;
    private readonly Func<int> scaleCallbackFunc;
    private readonly Type expectedType;
    private readonly string queueName;
    private string consumerTag;

    public TimeSpan CheckAliveFrequency { get; set; }

    public Task ConsumerWorkerTask { get; private set; }

    public bool ModelIsClosed
    {
        get { return this.model.IsClosed; }
    }

    public RabbitMQConsumerWorker(IConnection connection, string queueName, IMessageProcessingWorker messageProcessingWorker, IMessageRejectionHandler messageRejectionHandler, Func<int> scaleCallbackFunc, Type expectedType, CancellationToken parentToken)
    {
        Contract.Requires(connection != null, "connection is required");
        Contract.Requires(string.IsNullOrWhiteSpace(queueName) == false, "queueName is required");
        Contract.Requires(messageProcessingWorker != null, "messageProcessingWorker is required");
        Contract.Requires(scaleCallbackFunc != null, "scaleCallbackFunc is required");

        this.queueName = queueName;
        this.model = connection.CreateModel();
        this.model.BasicQos(0, 1, false);
        this.consumer = new EventingBasicConsumer(this.model);
        this.messageProcessingWorker = messageProcessingWorker;
        this.messageRejectionHandler = messageRejectionHandler;
        this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(parentToken);
        this.scaleCallbackFunc = scaleCallbackFunc;
        this.expectedType = expectedType;
        this.CheckAliveFrequency = new TimeSpan(0, 0, 10);


        this.consumer.Received += this.OnMessage;
        this.consumer.Registered += this.Consumer_Registered;
        this.consumer.Shutdown += this.Consumer_Shutdown;
        this.consumer.Unregistered += this.Consumer_Unregistered;
    }

    private void Consumer_Unregistered(object sender, ConsumerEventArgs e)
    {
    }

    private void Consumer_Shutdown(object sender, ShutdownEventArgs e)
    {
    }

    private void Consumer_Registered(object sender, ConsumerEventArgs e)
    {
    }

    private void OnMessage(object sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        if (!this.model.IsOpen)
        {
            //Dispose ConsumerWorker
            this.Dispose();

            //Throw AlreadyClosedException (model is already closed)
            throw new RabbitMQ.Client.Exceptions.AlreadyClosedException(this.model.CloseReason);
        }

        byte[] bodyBytes = basicDeliverEventArgs.Body.ToArray();
        string messageBody = Encoding.UTF8.GetString(bodyBytes);

        object messageObject = null;

        try
        {
            messageObject = Newtonsoft.Json.JsonConvert.DeserializeObject(messageBody, this.expectedType);
        }
        catch (Exception exception)
        {
            var deserializationException = new DeserializationException("Unable to deserialize data.", exception)
            {
                SerializedDataString = messageBody,
                SerializedDataBinary = bodyBytes,
                QueueName = this.queueName
            };
            //Pass DeserializationException to RejectionHandler
            this.messageRejectionHandler.OnRejection(deserializationException);

            //Remove message from queue after RejectionHandler dealt with it
            this.Nack(basicDeliverEventArgs.DeliveryTag, false);
        }

        //If message has been successfully deserialized and messageObject is populated
        if (messageObject != null)
        {
            //Create messageFeedbackSender instance with corresponding model and deliveryTag
            IMessageFeedbackSender messageFeedbackSender = new RabbitMQMessageFeedbackSender(model, basicDeliverEventArgs.DeliveryTag);

            try
            {
                //Call given messageProcessingWorker's OnMessage method to proceed with message processing
                messageProcessingWorker.OnMessage(messageObject, messageFeedbackSender);

                //If message has been processed with no errors but no Acknoledgement has been given
                if (!messageFeedbackSender.MessageAcknoledged)
                {
                    //Acknoledge message
                    this.Ack(basicDeliverEventArgs.DeliveryTag);
                }
            }
            catch (Exception)
            {
                //If something went wrong with message processing and message hasn't been acknoledged yet
                if (!messageFeedbackSender.MessageAcknoledged)
                {
                    //Negatively Acknoledge message, asking for requeue
                    this.Nack(basicDeliverEventArgs.DeliveryTag, true);
                }
                //Rethrow catched Exception
                throw;
            }
        }


    }

    public void DoConsume()
    {
        this.consumerTag = this.model.BasicConsume(this.queueName, false, this.consumer);

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }


    public void Ack(ulong deliveryTag)
    {
        model.BasicAck(deliveryTag, false);
    }

    public void Nack(ulong deliveryTag, bool requeue = false)
    {
        model.BasicNack(deliveryTag, false, requeue);
    }

    public void Dispose()
    {
        this.model.BasicCancel(this.consumerTag);
        this.model.Dispose();
    }
}