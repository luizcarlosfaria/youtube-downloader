using System;
using System.Collections.Generic;

namespace DevWeek.Architecture.Services
{
	public class MessageEnvelope
	{
		public Dictionary<string, object> Arguments { get; set; }

		public object ReturnValue { get; set; }

		public Exception Exception { get; set; }
	}
}