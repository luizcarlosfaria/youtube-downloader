//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace DevWeek.Architecture.Workflow
//{
//    //string    IComparable, ICloneable,    IConvertible,
//    //int32     IComparable, IFormattable,  IConvertible,

//    public abstract class State : IComparable
//    {
//        public string StateName { get; set; }

//        public abstract IComparable GetValue();

//        public abstract int CompareTo(object obj);
//    }

//    public abstract class State<StateType> : State
//    where StateType : IComparable
//    {
//        public StateType StateValue { get; set; }

//        public override IComparable GetValue()
//        {
//            return this.StateValue;
//        }

//        public override int CompareTo(object obj)
//        {
//            IComparable value = this.GetValue();
//            State rightState = (State)obj;
//            return value.CompareTo(rightState.GetValue());
//        }

//    }

//}