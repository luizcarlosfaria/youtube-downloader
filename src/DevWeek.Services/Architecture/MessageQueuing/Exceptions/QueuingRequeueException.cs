using System;

namespace DevWeek.Architecture.MessageQueuing.Exceptions
{
	public class QueuingRequeueException : BaseQueuingException
	{
		public QueuingRequeueException()
			: base()
		{
		}

		public QueuingRequeueException(string message)
			: base(message)
		{
		}

		public QueuingRequeueException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}