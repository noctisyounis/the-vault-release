using UnityEngine;

namespace Universe.Leaderboard.Runtime
{
	public class EntryDisplayer : UBehaviour
	{
		#region Exposed

		[Header("References")] 
		public UTextMeshPro m_rankDisplay;
		public UTextMeshPro m_nameDisplay;
		public UTextMeshPro m_scoreDisplay;
		public UTextMeshPro m_favouriteTarget;

		[Header("Settings")]
		public Entry m_value;
		public Color m_favouriteColor;

		public Entry Value
		{
			get => m_value;
			set
			{
				m_value = value;
				RefreshDisplay();
			}
		}

		#endregion


		#region Unity API

		public override void Awake()
			=> RefreshDisplay();

		#endregion


		#region Main

		public void RefreshDisplay()
		{
			var rank = Value.m_rank;
			var userName = Value.m_name;
			var score = Value.m_score;
			var favourite = Value.m_favourite;

			m_rankDisplay.m_text = $"{rank}";
			m_nameDisplay.m_text = userName;
			m_scoreDisplay.m_text = ScoreDisplayConvertion(score);
			m_favouriteTarget.m_overrideFontColor = favourite;
			
			m_rankDisplay.Refresh();
			m_nameDisplay.Refresh();
			m_scoreDisplay.Refresh();
			m_favouriteTarget.Refresh();
			if (!favourite) return;

			m_favouriteTarget.m_fontColor = m_favouriteColor;
			m_favouriteTarget.Refresh();
		}

		protected virtual string ScoreDisplayConvertion(long score)
			=> $"{score}";

		#endregion
	}
}
