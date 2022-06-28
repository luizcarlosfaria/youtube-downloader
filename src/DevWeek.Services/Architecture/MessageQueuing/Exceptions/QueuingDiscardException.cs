using System;

namespace DevWeek.Architecture.MessageQueuing.Exceptions;

public class QueuingDiscardException : BaseQueuingException
{
    public QueuingDiscardException()
        : base()
    {
    }

    public QueuingDiscardException(string message)
        : base(message)
    {
    }

    public QueuingDiscardException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}