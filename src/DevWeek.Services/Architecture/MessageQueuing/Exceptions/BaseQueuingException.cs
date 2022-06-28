using System;

namespace DevWeek.Architecture.MessageQueuing.Exceptions;

public class BaseQueuingException : Exception
{
    public BaseQueuingException()
        : base()
    {
    }

    public BaseQueuingException(string message)
        : base(message)
    {
    }

    public BaseQueuingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}