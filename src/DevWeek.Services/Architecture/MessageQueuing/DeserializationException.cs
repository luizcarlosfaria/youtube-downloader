using System;

namespace DevWeek.Architecture.MessageQueuing;

[Serializable]
public class DeserializationException : RejectionException
{
    public DeserializationException()
        : base()
    {
    }

    public DeserializationException(string message)
        : base(message)
    {
    }

    public DeserializationException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected DeserializationException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }

    public string SerializedDataString { get; set; }

    public ReadOnlyMemory<byte> SerializedDataBinary { get; set; }
}