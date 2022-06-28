using System;
using System.Runtime.Serialization;

namespace DevWeek.Architecture.Workflow.QueuedWorkFlow.Exceptions;

[Serializable]
public class FlowRetryException : DevWeek.Architecture.Business.BusinessException
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public FlowRetryException()
    {
    }

    public FlowRetryException(string message)
        : base(message)
    {
    }

    public FlowRetryException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected FlowRetryException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}