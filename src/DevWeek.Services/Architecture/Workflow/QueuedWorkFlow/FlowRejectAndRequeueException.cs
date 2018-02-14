using System;
using System.Runtime.Serialization;

namespace DevWeek.Architecture.Workflow.QueuedWorkFlow
{
	[Serializable]
	public class FlowRejectAndRequeueException : DevWeek.Architecture.Business.BusinessException
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public FlowRejectAndRequeueException()
		{
		}

		public FlowRejectAndRequeueException(string message)
			: base(message)
		{
		}

		public FlowRejectAndRequeueException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected FlowRejectAndRequeueException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}