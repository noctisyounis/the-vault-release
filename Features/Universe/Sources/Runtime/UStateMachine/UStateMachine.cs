using System;
using System.Collections.Generic;

namespace Universe.StateMachine.Runtime
{
	public class UStateMachine
	{
		#region Public Members

		public string 		m_name;
		public List<UState> 	m_states;

		#endregion


		#region Events

		public event Action<UState> OnStateEnter;
		public event Action<UState> OnStateExit;

		#endregion

		#region Constructor

		public UStateMachine(string name, UBehaviour owner)
		{
			m_name 		= name;
			_owner 		= owner;

			m_states 	= new List<UState>();
		}

		#endregion


		#region Main

		public void Tick()
		{
			_currentState.Tick();
		}

		public void FixedTick()
		{
			_currentState.FixedTick();
		}

		public void LateTick()
		{
			_currentState.LateTick();
		}

		public UState AddState(string name, UCommand command, bool allowDuplicate)
		{
			var newState = new UState(this, name, command);

			return AddState(newState, allowDuplicate);
		}

		public UState AddState(UState state, bool allowDuplicate)
		{
			if(!allowDuplicate && m_states.Contains(state)) return null;
			
			state.SetOwner(_owner);
			m_states.Add(state);

			return state;
		}

		public void RemoveState(UState state)
		{
			if(!m_states.Contains(state)) return;

			m_states.Remove(state);
		}

		public void SetState(UState state)
		{
			if(state == null) 				return;
			if(!m_states.Contains(state)) 	throw new Exception($"{state.m_name} doesnt exists in {m_name} of {_owner}");
			if(state.Equals(_currentState)) return;

			OnStateExit?.Invoke(_currentState);

			_currentState = state;

			OnStateEnter?.Invoke(_currentState);
		}

		public void AddTransition(UState source, UState destination, Func<bool> condition)
		{
			if(source 		== null) return;
			if(destination 	== null) return;
			if(condition 	== null) return;

			source.AddTransition(condition, destination);
		}

		public void RemoveTransition(UState source, UState destination)
		{
			if(source 		== null) return;
			if(destination 	== null) return;

			source.RemoveTransition(destination);
		}

		#endregion


		#region Private Members

		private UBehaviour 	_owner;
		private UState 		_currentState;

		#endregion
	}
}