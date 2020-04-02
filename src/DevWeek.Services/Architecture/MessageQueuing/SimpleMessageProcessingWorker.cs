using DevWeek.Architecture.Extensions;
using DevWeek.Architecture.Services;
using Oragon.Spring.Objects.Factory;
using Oragon.Spring.Objects.Factory.Attributes;
using Oragon.Spring.Objects.Support;
using System;
using System.Reflection;

namespace DevWeek.Architecture.MessageQueuing
{
	public class SimpleMessageProcessingWorker : IService, IMessageProcessingWorker, IInitializingObject, IDisposable
	{
		[Required]
		public IQueueClient QueueClient { get; set; }

		[Required]
		public bool AutoStartup { get; set; }

		[Required]
		public string QueueName { get; set; }

		[Required]
		public object Service { get; set; }

		[Required]
		public string ServiceMethod { get; set; }

		[Required]
		public IConsumerCountManager ConsumerCountManager { get; set; }

		[Required]
		public IMessageRejectionHandler MessageRejectionHandler { get; set; }

		public IQueueConsumer Consumer { get; private set; }

		public string Name { get { return "SimpleMessageListenerContainer"; } }

		public void AfterPropertiesSet()
		{
			if (this.AutoStartup)
			{
				this.Start();
			}
		}

		public void Start()
		{
			if (this.Consumer == null)
			{
				MethodInvoker methodInvoker = new MethodInvoker();
				methodInvoker.TargetObject = this.Service;
				methodInvoker.TargetMethod = this.ServiceMethod;
				methodInvoker.Arguments = new object[] { null };
				methodInvoker.Prepare();
				MethodInfo methodInfo = methodInvoker.GetPreparedMethod();
				Type inType = methodInfo.GetUniqueAndExpectedInputParameterType();
				this.Consumer = this.QueueClient.GetConsumer(this.QueueName, this.ConsumerCountManager, this, inType, this.MessageRejectionHandler);
				this.Consumer.Start();
			}
		}

		public void Stop()
		{
			if (this.Consumer != null)
			{
				this.Consumer.Stop();
				this.Consumer = null;
				this.QueueClient.Dispose();
			}
		}

		public virtual void OnMessage(object message, IMessageFeedbackSender feedbackSender)
		{
			MethodInvoker methodInvoker = new MethodInvoker();
			methodInvoker.TargetObject = this.Service;
			methodInvoker.TargetMethod = this.ServiceMethod;
			methodInvoker.Arguments = new object[] { message };
			methodInvoker.Prepare();
			try
			{
				object returnValue = methodInvoker.Invoke();
				feedbackSender.Ack();
			}
			catch (Exception)
			{
				feedbackSender.Nack(true);
			}
		}

		public void Dispose()
		{
			this.Stop();
		}
	}
}