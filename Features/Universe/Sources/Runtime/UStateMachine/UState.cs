using System;
using System.Collections.Generic;

namespace Universe.StateMachine.Runtime
{
	public sealed class UState
	{
		#region Public Members

		public string m_name;

		#endregion


		#region Public Properties

		public UCommand Command {get; set;}

		#endregion


		#region Constructor

		public UState(UStateMachine stateMachine, string name, UCommand command)
		{
			_stateMachine 	= stateMachine;
			m_name 			= name;
			_command 		= command;
			_transitions 	= new List<UTransition>();

			_stateMachine.OnStateEnter 	+= OnEnter;
			_stateMachine.OnStateExit 	+= OnExit;
		}

		#endregion


		#region Main

		private void OnEnter(UState state)
		{
			if(!state.Equals(this)) return;

			_started = true;

			if(!_command) return;

			_command.Initialize(_owner);
		}

		public void Tick()
		{
			if(!_started) OnEnter(this);
			if(!_command) return;

			_command.Execute(_owner);
		}

		public void FixedTick()
		{
			if(!_command) return;
			
			_command.FixedExecute(_owner);
		}

		public void LateTick()
		{
			if(_command) _command.LateExecute(_owner);

			EvaluateTransitions();
		}
		
		private void OnExit(UState state)
		{
			if(!Equals(state)) return;

			_started = false;

			if(!_command) return;

			_command.Terminate(_owner);
		}

		public void SetOwner(UBehaviour owner)
		{
			_owner = owner;
		}

		public void AddTransition(Func<bool> condition, UState destination)
		{
			var transition = new UTransition(condition, destination, m_name);

			AddTransition(transition);
		}

		public void AddTransition(UTransition transition)
		{
			_transitions.Add(transition);
		}

		public void RemoveTransition(UState destination)
		{
			var transition = new UTransition(null, destination, m_name);

			RemoveTransition(transition);
		}

		public void RemoveTransition(UTransition transition)
        {
            if (!HasTransition(transition)) return;

            _transitions.Remove(transition);
        }


        #endregion


        #region Utils

        private void EvaluateTransitions()
		{
			for (int i = 0; i < _transitions.Count; i++)
			{
				var transition 		= _transitions[i];
				var canChangeState 	= transition.Evaluate();

				if(!canChangeState) continue;
				
				var nextState 		= transition.m_destination;

				_stateMachine.SetState(nextState);

				return;
			}
		}

        private bool HasTransition(UTransition transition) => _transitions.Contains(transition);

		#endregion


		#region Private Members

		private bool 				_started;
		private UStateMachine 		_stateMachine;
		private UBehaviour 			_owner;
		private UCommand 			_command;
		private List<UTransition> 	_transitions;

		#endregion
	}
}