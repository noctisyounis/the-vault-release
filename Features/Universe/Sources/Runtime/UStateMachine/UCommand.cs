using UnityEngine;

namespace Universe.StateMachine.Runtime
{
	public abstract class UCommand : ScriptableObject
	{
		#region Main

		public abstract void Initialize(UBehaviour target = null);
		public abstract void Execute(UBehaviour target = null);
		public abstract void FixedExecute(UBehaviour target = null);
		public abstract void LateExecute(UBehaviour target = null);
		public abstract void Terminate(UBehaviour target = null);

		#endregion
	}
}