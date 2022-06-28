using System;
using System.Collections.Generic;

namespace DevWeek.Architecture.Workflow;

public class StateComprarer<StateType> : IEqualityComparer<StateType>
           where StateType : IComparable
{
    public bool Equals(StateType x, StateType y)
    {
        int equalValue = 0;
        var defValue = default(StateType);
        if (x.CompareTo(defValue) == equalValue && y.CompareTo(defValue) == equalValue)
            return true;
        else if (x.CompareTo(defValue) != equalValue && y.CompareTo(defValue) != equalValue)
            return x.CompareTo(y) == equalValue;
        else
            return false;
    }

    public int GetHashCode(StateType obj)
    {
        return obj.GetHashCode();
    }
}