using DevWeek.Architecture.MessageQueuing.Exceptions;
using Spring.Objects.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevWeek.Architecture.MessageQueuing;

	public class AdvancedMessageProcessingWorker : SimpleMessageProcessingWorker
	{
		public int InvokeRetryCount { get; set; }

		public int InvokeRetryWaitMilliseconds { get; set; }

		public ExceptionHandlingStrategy ExceptionHandlingStrategy { get; set; }

		public AdvancedMessageProcessingWorker()
		{
			this.InvokeRetryCount = 1;
			this.InvokeRetryWaitMilliseconds = 0;
			this.ExceptionHandlingStrategy = ExceptionHandlingStrategy.Requeue;
		}

		public override void OnMessage(object message, IMessageFeedbackSender feedbackSender)
		{
			MethodInvoker methodInvoker = new MethodInvoker();
			methodInvoker.TargetObject = this.Service;
			methodInvoker.TargetMethod = this.ServiceMethod;
			methodInvoker.Arguments = new object[] { message };
			methodInvoker.Prepare();

			bool invocationSuccess = false;
			var exceptions = new List<Exception>();

			int tryCount = 0;

			while (tryCount == 0 || (!invocationSuccess && this.ShouldRetry(tryCount, exceptions)))
			{
				if (tryCount > 0 && this.InvokeRetryWaitMilliseconds > 0)
				{
					Thread.Sleep(this.InvokeRetryWaitMilliseconds);
				}

				tryCount++;

				this.TryInvoke(methodInvoker, exceptions, out invocationSuccess);
			}
				
			if(invocationSuccess)
			{
				feedbackSender.Ack();
			}
			else if(this.ShouldRequeue(exceptions))
			{
				feedbackSender.Nack(true);
			}
			else
			{
				feedbackSender.Nack(false);
			}
		}

		private ExceptionHandlingStrategy? GetStrategyByExceptions(List<Exception> exceptions)
		{
			if(exceptions.Any())
			{
				if (exceptions.Last() is QueuingRetryException || exceptions.Last().InnerException is QueuingRetryException)
				{
					return ExceptionHandlingStrategy.Retry;
				}
				else if (exceptions.Last() is QueuingRequeueException || exceptions.Last().InnerException is QueuingRequeueException)
				{
					return ExceptionHandlingStrategy.Requeue;
				}
				else if (exceptions.Last() is QueuingDiscardException || exceptions.Last().InnerException is QueuingDiscardException)
				{
					return ExceptionHandlingStrategy.Discard;
				}
			}

			return null;
		}

		private bool ShouldRetry(int tryCount, List<Exception> exceptions)
		{
			if(tryCount >= this.InvokeRetryCount)
			{
				return false;
			}

			ExceptionHandlingStrategy? strategyByExceptions = this.GetStrategyByExceptions(exceptions);

			if (strategyByExceptions != null)
			{
				if(strategyByExceptions == ExceptionHandlingStrategy.Retry)
				{
					return true;
				}
				else if(strategyByExceptions == ExceptionHandlingStrategy.Discard)
				{
					return false;
				}
			}

			if(this.ExceptionHandlingStrategy == ExceptionHandlingStrategy.Retry)
			{
				return true;
			}

			return false;
		}

		private bool ShouldRequeue(List<Exception> exceptions)
		{
			ExceptionHandlingStrategy? strategyByExceptions = this.GetStrategyByExceptions(exceptions);

			if(strategyByExceptions != null)
			{
				if(strategyByExceptions == ExceptionHandlingStrategy.Requeue)
				{
					return true;
				}
				else if(strategyByExceptions == ExceptionHandlingStrategy.Discard)
				{
					return false;
				}
			}

			if(this.ExceptionHandlingStrategy == ExceptionHandlingStrategy.Requeue)
			{
				return true;
			}

			return false;
		}

		private void TryInvoke(MethodInvoker methodInvoker, List<Exception> exceptions, out bool invocationSuccess)
		{
			invocationSuccess = false;

			try
			{
				methodInvoker.Invoke();

				invocationSuccess = true;
			}
			catch (Exception exception)
			{
				exceptions.Add(exception);
			}
		}
	}
