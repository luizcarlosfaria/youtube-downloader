using DevWeek.Architecture.Extensions;
using DevWeek.Architecture.MessageQueuing;
using DevWeek.Architecture.Services;
using Spring.Objects.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DevWeek.Architecture.Workflow.QueuedWorkFlow;

public class QueuedStateMachine : StateMachine<QueuedTransition, string>, IInitializingObject, IObjectNameAware, IService
{
    protected IQueueClient QueueClient { get; set; }

    public IMessageRejectionHandler MessageRejectionHandler { get; set; }

    public bool CreateZombieQueues { get; set; }

    private List<QueuedTransition> GetAvailableTransitions()
    {
        return this.Transitions.Where(it => it.ConsumerCountManager.MaxConcurrentConsumers != 0).ToList();
    }

    private List<QueuedTransition> GetAllTransitions()
    {
        return this.Transitions;
    }

    public QueuedStateMachine()
    {
    }

    private void InitializeBroker()
    {
        foreach (QueuedTransition queuedTransition in this.GetAllTransitions())
        {
            this.ConfigureBroker(queuedTransition);
        }
    }

    private void InitializeServices()
    {
        foreach (QueuedTransition queuedTransition in this.GetAvailableTransitions())
        {
            queuedTransition.InitializeTransition();
        }
    }

    private void ConfigureBroker(QueuedTransition queuedTransition)
    {
        if (queuedTransition.ConsumerCountManager.MaxConcurrentConsumers > 0)
        {
            MethodInfo methodInfo = Spring.Util.ReflectionUtils.GetMethod(queuedTransition.Service.GetType(), queuedTransition.ServiceMethod, new Type[] { });
            if (methodInfo == null)
                throw new InvalidOperationException(string.Format("Service Method '{0}' of transition cannot be found", queuedTransition.ServiceMethod));

            QueuedWorkflowMessageProcessingWorker messageListenerAdapter = new QueuedWorkflowMessageProcessingWorker(
                    queueClient: this.QueueClient,
                    listenerObject: queuedTransition.Service,
                    listenerMethod: methodInfo,
                    queuedTransition: queuedTransition,
                    nextQueuedTransition: this.GetPossibleTransitions(queuedTransition.Destination).FirstOrDefault(),
                    createZombieQueues: this.CreateZombieQueues
                    );

            queuedTransition.QueueConsumer = this.QueueClient.GetConsumer(messageListenerAdapter.ReceiveRoute.QueueName, queuedTransition.ConsumerCountManager, messageListenerAdapter, methodInfo.GetUniqueAndExpectedInputParameterType(), this.MessageRejectionHandler);
        }
    }

    public void AfterPropertiesSet()
    {
        this.InitializeBroker();
        this.InitializeServices();
    }

    public bool IsRunning
    {
        get { return this.GetAvailableTransitions().Any(it => it.QueueConsumer.GetConsumerCount() > 0); }
    }

    public void Start()
    {
        this.GetAvailableTransitions().ForEach(it => it.QueueConsumer.Start());
    }

    public void Stop()
    {
        this.GetAvailableTransitions().AsParallel().ForAll(it => it.QueueConsumer.Stop());
        this.QueueClient.Dispose();
    }

    public string ObjectName { get; set; }

    public string Name
    {
        get { return "QueuedStateMachine"; }
    }
}