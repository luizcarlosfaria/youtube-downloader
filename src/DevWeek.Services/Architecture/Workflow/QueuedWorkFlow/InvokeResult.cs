using System;

namespace DevWeek.Architecture.Workflow.QueuedWorkFlow;

public class InvokeResult
{
    public object ReturnedValue { get; set; }

    public Exception Exception { get; set; }

    public bool IsMissing
    {
        get
        {
            return (this.ReturnedValue is System.Reflection.Missing);
        }
    }

    public bool HasValue
    {
        get
        {
            return (this.ReturnedValue != null);
        }
    }

    public bool Success
    {
        get
        {
            return (this.Exception == null);
        }
    }
}