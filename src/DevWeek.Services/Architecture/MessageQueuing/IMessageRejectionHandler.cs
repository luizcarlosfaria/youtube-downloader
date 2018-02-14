namespace DevWeek.Architecture.MessageQueuing
{
	public interface IMessageRejectionHandler
	{
		void OnRejection(RejectionException exception);
	}
}