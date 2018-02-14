namespace DevWeek.Architecture.MessageQueuing
{
	public interface IMessageProcessingWorker
	{
		void OnMessage(object message, IMessageFeedbackSender feedbackSender);
	}
}