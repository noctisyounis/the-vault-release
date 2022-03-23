using System;
using System.Collections.Generic;

namespace Universe.StateMachine.Runtime
{
	[Serializable]
	public class UTransition
	{
		#region Public Members

		public string 	m_name;
		public UState 	m_destination;

        #endregion


		#region Constructor

		public UTransition(Func<bool> condition, UState destination) : 
			this(condition, destination, ""){}

		public UTransition(Func<bool> condition, UState destination, string sourceName)
		{
			m_destination 	= destination;
			_condition 		= condition;
			m_name 			= $"{sourceName}To{destination.m_name}";
		}

		#endregion


		#region Main

		public bool Evaluate()
		{
			return _condition();
		}

        #endregion


        #region Utils

		public override bool Equals(object obj)
        {
			if(!(obj is UTransition transition)) return false;
			return transition.m_name.Equals(m_name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_name, m_destination, _condition);
        }

        #endregion


        #region Private Members

        private Func<bool> _condition;

		#endregion
    }
}