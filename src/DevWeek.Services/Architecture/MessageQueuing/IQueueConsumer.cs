using System;

namespace DevWeek.Architecture.MessageQueuing;

public interface IQueueConsumer : IDisposable
{
    IConsumerCountManager ConsumerCountManager { get; }

    void Start();

    void Stop();

    uint GetMessageCount();

    uint GetConsumerCount();

    IMessageProcessingWorker MessageProcessingWorker { get; set; }
}