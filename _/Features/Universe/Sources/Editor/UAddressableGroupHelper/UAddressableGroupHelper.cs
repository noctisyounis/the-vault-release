using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

using static UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject;

namespace Universe
{
	public class UAddressableGroupHelper : ScriptableObject
	{
		#region Public 

		public AddressableAssetGroupTemplate m_template;
		public string m_groupName;
		public AddressableAssetGroup m_group;

		#endregion


		#region Main

		public AddressableAssetGroup GenerateNewGroup()
		{
			m_group = Settings.FindGroup(FindByNameEquality);

			if(m_group) return m_group;
			
			m_group = Settings.CreateGroup(m_groupName, false, false, true, null, m_template.GetTypes());
			GetSettings(false);

			return m_group;
		}

		public bool FindByNameEquality(AddressableAssetGroup other) => other.name.Equals(m_groupName);

		public void SetGroupAsDefault()
		{
			m_group = TryToFindGroup();

			if(!m_group) return;

            Settings.DefaultGroup = m_group;
		}

        private AddressableAssetGroup TryToFindGroup()
        {
            var group = Settings.FindGroup(FindByNameEquality);

            if (group) return group;
            
            Debug.LogError($"No existing group found, please create one before setting it as default.");
			return null;
        }

        #endregion
	}
}