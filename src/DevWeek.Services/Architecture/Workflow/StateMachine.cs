using System;
using System.Collections.Generic;
using System.Linq;

namespace DevWeek.Architecture.Workflow;

	public class StateMachine<TransitionType, StateType>
		where StateType : IComparable
		where TransitionType : Transition<StateType>
	{
		private StateComprarer<StateType> StateComprarer;
		private TransitionComprarer<TransitionType, StateType> TransitionComparer;

		public List<TransitionType> Transitions { get; set; }

		public StateMachine()
		{
			this.StateComprarer = new StateComprarer<StateType>();
			this.TransitionComparer = new TransitionComprarer<TransitionType, StateType>(this.StateComprarer);
		}

		#region Queries

		public IEnumerable<StateType> GetAllStates()
		{
			return
				this.Transitions.Select(it => it.Origin)
				.Union(this.Transitions.Select(it => it.Destination))
				.Distinct(this.StateComprarer);
		}

		public IEnumerable<TransitionType> GetPossibleTransitions(StateType stateValue)
		{
			return this.Transitions.Where(it => this.StateComprarer.Equals(it.Origin, stateValue));
		}

		public IEnumerable<StateType> GetPossibleDestinations(StateType stateValue)
		{
			return this.GetPossibleTransitions(stateValue).Select(it => it.Destination);
		}

		public TransitionType GetTransition(StateType sourceValue, StateType targetValue)
		{
			return this.Transitions.Where(it =>
						it.Origin.Equals(sourceValue)
						&&
						it.Destination.Equals(targetValue)
			).FirstOrDefault();
		}

		public IEnumerable<StateType> GetFinalStates()
		{
			IEnumerable<StateType> returnValue = this.GetFinalTransitions()
										.Select(it => it.Destination)
										.Distinct(this.StateComprarer);
			return returnValue;
		}

		public IEnumerable<TransitionType> GetFinalTransitions()
		{
			IEnumerable<TransitionType> returnValue = this.Transitions.Where(it =>
													this.Transitions.Any(it2 => it2.Origin.Equals(it.Destination)) == false
										)
										.Distinct(this.TransitionComparer);
			return returnValue;
		}

		public IEnumerable<StateType> GetInitialStates()
		{
			IEnumerable<StateType> returnValue = this.GetInitialTransitions()
										.Select(it => it.Origin)
										.Distinct(this.StateComprarer);
			return returnValue;
		}

		public IEnumerable<TransitionType> GetInitialTransitions()
		{
			IEnumerable<TransitionType> returnValue = this.Transitions.Where(it =>
													this.Transitions.Any(it2 => it2.Destination.Equals(it.Origin)) == false
										)
										.Distinct(this.TransitionComparer);
			return returnValue;
		}

		#endregion Queries
	}