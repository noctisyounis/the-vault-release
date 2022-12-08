using System.Collections.Generic;
using UnityEngine;

namespace Universe.SceneTask.Runtime
{
	public class LevelData : UniverseScriptableObject
	{
		#region Exposed

		public TaskData m_player;
		public TaskData m_audio;

		public List<SituationData> Situations => 
			_situations ??= new();

		#endregion


		#region Public API

		public int IndexOf( SituationData situation )
			=> Situations.IndexOf( situation );
		
		public void AddSituation(SituationData situation)
		{
			if (Situations.Contains(situation)) return;
			
	        Situations.Add(situation);
		}

		public SituationData GetSituation( int index )
		{
			if( !Situations.GreaterThan( index ) )
				return null;

			return Situations[index];
		}

        #endregion
        
        
        #region Private

        [SerializeField] private List<SituationData> _situations;

        #endregion
	}
}