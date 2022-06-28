using System;

namespace DevWeek.Architecture.Business;

/// <summary>
/// Define uma exception não gerenciada tratada pelo mecanismo de exception replacement
/// </summary>
[Serializable]
public class UndefinedException : BusinessException
{
    public UndefinedException()
    {
    }

    public UndefinedException(string message)
        : base(message)
    {
    }

    public UndefinedException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected UndefinedException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}