using RabbitMQ.Client;
using Spring.Objects.Factory.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Architecture.MessageQueuing;

public class RabbitMQClient : IQueueClient
{

    [Required]
    private RabbitMQConnectionPool ConnectionPool { get; set; }

    public void Publish<T>(string exchangeName, string routingKey, T content)
    {
        string serializedContent = Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            IBasicProperties props = model.CreateBasicProperties();
            props.DeliveryMode = 2;
            byte[] payload = Encoding.UTF8.GetBytes(serializedContent);
            model.BasicPublish(exchangeName, routingKey, props, payload);
        }
    }

    public void BatchPublish<T>(string exchangeName, string routingKey, IEnumerable<T> contentList)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            IBasicProperties props = model.CreateBasicProperties();
            props.DeliveryMode = 2;

            foreach (var content in contentList)
            {
                string serializedContent = Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);

                byte[] payload = Encoding.UTF8.GetBytes(serializedContent);
                model.BasicPublish(exchangeName, routingKey, props, payload);
            }
        }
    }

    public void BatchPublishTransactional<T>(string exchangeName, string routingKey, IEnumerable<T> contentList)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            try
            {
                model.TxSelect();

                IBasicProperties props = model.CreateBasicProperties();
                props.DeliveryMode = 2;

                foreach (var content in contentList)
                {
                    string serializedContent = Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);

                    byte[] payload = Encoding.UTF8.GetBytes(serializedContent);
                    model.BasicPublish(exchangeName, routingKey, props, payload);
                }

                model.TxCommit();
            }
            catch (Exception)
            {
                if (model.IsOpen)
                {
                    model.TxRollback();
                }

                throw;
            }
        }
    }

    public void QueueDeclare(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            model.QueueDeclare(queueName, durable, exclusive, autoDelete, arguments);
        }
    }

    public void QueueDeclarePassive(string queueName)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            model.QueueDeclarePassive(queueName);
        }
    }

    public void QueueBind(string queueName, string exchangeName, string routingKey)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            model.QueueBind(queueName, exchangeName, routingKey);
        }
    }

    public void ExchangeDeclare(string exchangeName, bool passive = false)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            if (passive)
            {
                model.ExchangeDeclarePassive(exchangeName);
            }
            else
            {
                model.ExchangeDeclare(exchangeName, ExchangeType.Topic, true);
            }
        }
    }

    public bool QueueExists(string queueName)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            try
            {
                model.QueueDeclarePassive(queueName);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

    public void EnsureQueueExists(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
    {
        if (!this.QueueExists(queueName))
        {
            this.QueueDeclare(queueName, durable, exclusive, autoDelete, arguments);
        }
    }

    public uint QueuePurge(string queueName)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            uint returnValue = model.QueuePurge(queueName);
            return returnValue;
        }
    }

    public uint GetMessageCount(string queueName)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            QueueDeclareOk queueDeclareOk = model.QueueDeclarePassive(queueName);

            return queueDeclareOk.MessageCount;
        }
    }

    public uint GetConsumerCount(string queueName)
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            QueueDeclareOk queueDeclareOk = model.QueueDeclarePassive(queueName);

            return queueDeclareOk.ConsumerCount;
        }
    }

    public IQueueConsumer GetConsumer(string queueName, IConsumerCountManager consumerCountManager, IMessageProcessingWorker messageProcessingWorker, Type expectedType, IMessageRejectionHandler messageRejectionHandler)
    {
        return new RabbitMQConsumer(
            connectionPool: this.ConnectionPool,
            queueName: queueName,
            expectedType: expectedType,
            messageProcessingWorker: messageProcessingWorker,
            consumerCountManager: consumerCountManager,
            messageRejectionHandler: messageRejectionHandler);
    }

    public void Dispose()
    {
        if (this.ConnectionPool != null)
        {
            this.ConnectionPool.Dispose();
            this.ConnectionPool = null;
        }
    }
}