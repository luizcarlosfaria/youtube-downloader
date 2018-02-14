using System;
using System.Runtime.Serialization;

namespace DevWeek.Architecture.Services
{
	[Serializable]
	public class ServiceStartException : Exception
	{
		public ServiceStartException()
		{
		}

		public ServiceStartException(string message)
			: base(message)
		{
		}

		public ServiceStartException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ServiceStartException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}