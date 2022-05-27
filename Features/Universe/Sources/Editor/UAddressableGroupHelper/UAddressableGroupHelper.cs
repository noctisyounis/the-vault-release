using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

using static UnityEditor.AssetDatabase;
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
			m_group = TryToFindGroup();

			if(!m_group) 
				m_group = Settings.CreateGroup(m_groupName, false, false, true, null, m_template.GetTypes());

			EditorUtility.SetDirty(this);
			SaveAssetIfDirty(this);
			
			return m_group;
		}

		public void SetGroupAsDefault()
		{
			m_group = TryToFindGroup();

			if(!m_group) return;

            Settings.DefaultGroup = m_group;
		}

        public AddressableAssetGroup TryToFindGroup()
        {
            var group = Settings.FindGroup(FindByNameEquality);

            if (group) return group;
            
            Debug.LogError($"No existing group found", this);
			return null;
        }

        #endregion


		#region Utils
		public bool FindByNameEquality(AddressableAssetGroup other) => other.name.Equals(m_groupName);

		#endregion
	}
}