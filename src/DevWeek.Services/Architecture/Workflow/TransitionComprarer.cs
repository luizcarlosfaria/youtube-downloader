using System;
using System.Collections.Generic;

namespace DevWeek.Architecture.Workflow;

	public class TransitionComprarer<TransitionType, StateType> : IEqualityComparer<TransitionType>
		where StateType : IComparable
		where TransitionType : Transition<StateType>
	{
		private StateComprarer<StateType> StateComprarer;

		public TransitionComprarer()
			: this(new StateComprarer<StateType>())
		{
		}

		public TransitionComprarer(StateComprarer<StateType> stateComparer)
		{
			this.StateComprarer = stateComparer;
		}

		public bool Equals(TransitionType x, TransitionType y)
		{
			var defValue = default(TransitionType);
			if (x == defValue && y == defValue)
				return true;
			else if (x != defValue && y != defValue)
				return (
					this.StateComprarer.Equals(x.GetOrigin(), y.GetOrigin())
					&&
					this.StateComprarer.Equals(x.GetDestination(), y.GetDestination())
				);
			return false;
		}

		public int GetHashCode(TransitionType obj)
		{
			return string.Concat(obj.GetOrigin(), "|", obj.GetDestination()).GetHashCode();
		}
	}