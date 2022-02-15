using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universe.SceneTask.Runtime
{
	public class UCrashTest : UBehaviour
	{
		public AssetReference m_prefab;
		public int m_amount;

		#region Unity API

		public void OnGUI() 
		{
			GUILayout.Label($"{m_amount}");
			if(GUILayout.Button("Spawn 1000"))
			{
				Spawn1000();
			}
		}

		#endregion


		#region Main

		private void Spawn1000()
		{
			for (int i = 0; i < 1000; i++)
			{
				Spawn(m_prefab, Random.insideUnitSphere, Quaternion.identity, transform);
			}

			m_amount += 1000;
		}

		#endregion
	}
}