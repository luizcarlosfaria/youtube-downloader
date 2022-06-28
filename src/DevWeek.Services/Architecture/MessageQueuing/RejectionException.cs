using System;

namespace DevWeek.Architecture.MessageQueuing;

[Serializable]
public class RejectionException : Exception
{
    public RejectionException()
        : base()
    {
    }

    public RejectionException(string message)
        : base(message)
    {
    }

    public RejectionException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected RejectionException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }

    public string VirtualHost { get; set; }

    public string QueueName { get; set; }
}