using UnityEngine;
using UnityEngine.SceneManagement;


namespace Universe.SceneTask.Runtime
{
	public class UCrashTestItem : UBehaviour
	{
		#region Public

		public float m_angularSpeed;

		#endregion


		#region Main

		public override void Awake()
		{
			SceneManager.MoveGameObjectToScene(gameObject, Task.GetFocusScene());
			base.Awake();
		}

		public override void OnUpdate(float deltaTime)
		{
			transform.Rotate(Vector3.one * m_angularSpeed * deltaTime);
		}

		#endregion
	}
}