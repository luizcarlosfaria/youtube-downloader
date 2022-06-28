using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevWeek.Architecture.MessageQueuing;

public class RabbitMQConsumer : IQueueConsumer
{
    public RabbitMQConnectionPool ConnectionPool { get; private set; }

    public string QueueName { get; private set; }

    public Type ExpectedType { get; private set; }

    public IMessageProcessingWorker MessageProcessingWorker { get; set; }

    public IMessageRejectionHandler MessageRejectionHandler { get; set; }

    public IConsumerCountManager ConsumerCountManager { get { return this._consumerCountManager; } }

    private object _scalingAmountSyncLock = new object();


    private readonly CancellationTokenSource _cancellationTokenSource;

    private volatile int _scalingAmount;
    private volatile bool _isStopped;
    private volatile int _consumerWorkersCount;
    private volatile IConsumerCountManager _consumerCountManager;

    public RabbitMQConsumer(RabbitMQConnectionPool connectionPool, string queueName, Type expectedType, IMessageProcessingWorker messageProcessingWorker, IConsumerCountManager consumerCountManager, IMessageRejectionHandler messageRejectionHandler)
    {
        //Set using constructor parameters
        this.ConnectionPool = connectionPool;
        this.QueueName = queueName;
        this.ExpectedType = expectedType;
        this.MessageProcessingWorker = messageProcessingWorker;
        this._consumerCountManager = consumerCountManager;
        this.MessageRejectionHandler = messageRejectionHandler;

        //Set using default values
        this._consumerWorkersCount = 0;
        this._cancellationTokenSource = new CancellationTokenSource();
        this._isStopped = true;
    }

    public void Start()
    {
        this._isStopped = false;
        CancellationToken token = this._cancellationTokenSource.Token;

        var thread = new Thread(() => this.ManageConsumersLoop(token))
        {
            IsBackground = true
        };
        thread.Start();


        //Task.Factory.StartNew(() => this.ManageConsumersLoop(token), token);
    }

    public void Stop()
    {
        this._isStopped = true;
        lock (this._scalingAmountSyncLock)
        {
            this._scalingAmount = this._consumerWorkersCount * -1;
        }
        this._cancellationTokenSource.Cancel();
    }

    public void SetQueueName(string queueName)
    {
        if (queueName != null)
        {
            this.QueueName = queueName;
        }
    }

    public uint GetMessageCount()
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            return this.GetMessageCount(model);
        }
    }

    public uint GetConsumerCount()
    {
        using (IModel model = this.ConnectionPool.GetConnection().CreateModel())
        {
            return this.GetConsumerCount(model);
        }
    }

    protected virtual void ManageConsumersLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!this._isStopped)
            {
                QueueInfo queueInfo = this.CreateQueueInfo();
                this._scalingAmount = this._consumerCountManager.GetScalingAmount(queueInfo, this._consumerWorkersCount);
                int scalingAmount = this._scalingAmount;
                for (var i = 1; i <= scalingAmount; i++)
                {
                    this._scalingAmount--;
                    this._consumerWorkersCount++;

                    var thread = new Thread(() =>
                    {
                        try
                        {
                            using (IQueueConsumerWorker consumerWorker = this.CreateNewConsumerWorker(token))
                            {
                                consumerWorker.DoConsume();
                            }
                        }
                        catch (Exception exception)
                        {
                            System.Diagnostics.Trace.TraceError("DevWeek.Architecture.MessageQueuing.RabbitMQConsumer", exception.ToString(), "QueueName", this.QueueName);
                        }
                        finally
                        {
                            this._consumerWorkersCount--;
                            this._scalingAmount++;
                        }

                    })
                    {
                        IsBackground = true
                    };
                    thread.Start();
                }
            }

            Thread.Sleep(this._consumerCountManager.AutoscaleFrequency);
        }
    }

    #region Consumer CallBack Methods

    private RabbitMQConsumerWorker CreateNewConsumerWorker(CancellationToken parentToken)
    {
        var newConsumerWorker = new RabbitMQConsumerWorker(
            connection: this.ConnectionPool.GetConnection(),
            queueName: this.QueueName,
            messageProcessingWorker: this.MessageProcessingWorker,
            messageRejectionHandler: this.MessageRejectionHandler,
            scaleCallbackFunc: this.GetScalingAmount,
            expectedType: this.ExpectedType,
            parentToken: parentToken
        );

        return newConsumerWorker;
    }

    private int GetScalingAmount()
    {
        return this._scalingAmount;
    }

    #endregion Consumer CallBack Methods

    public void Dispose()
    {
        this.Stop();
    }

    #region Private Methods

    private QueueInfo CreateQueueInfo()
    {
        try
        {
            QueueInfo queueInfo = null;

            var connection = this.ConnectionPool.GetConnection();

            using (IModel model = connection.CreateModel())
            {
                queueInfo = new QueueInfo()
                {
                    QueueName = this.QueueName,
                    ConsumerCount = this.GetConsumerCount(model),
                    MessageCount = this.GetMessageCount(model)
                };
            }
            return queueInfo;
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }

    private uint GetMessageCount(IModel model)
    {
        QueueDeclareOk queueDeclareOk = model.QueueDeclarePassive(this.QueueName);
        return queueDeclareOk.MessageCount;
    }

    private uint GetConsumerCount(IModel model)
    {
        QueueDeclareOk queueDeclareOk = model.QueueDeclarePassive(this.QueueName);
        return queueDeclareOk.ConsumerCount;
    }

    #endregion Private Methods
}