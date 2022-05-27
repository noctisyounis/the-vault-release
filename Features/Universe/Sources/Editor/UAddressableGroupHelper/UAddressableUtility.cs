using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Universe
{
	static class UAddressableUtility
	{
		public static void RemoveAaEntry(AddressableAssetSettings aaSettings, string targetGUID)
        {
            var e = aaSettings.FindAssetEntry(targetGUID);
            
			if(e == null) return;
			
			var removedEntry = aaSettings.RemoveAssetEntry(targetGUID, false);

			aaSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, removedEntry, true, false);
        }

		public static void CreateAaEntry(AddressableAssetSettings aaSettings, string targetGUID, AddressableAssetGroup parentGroup)
        {
            if (parentGroup.ReadOnly)
            {
                Debug.LogError("Current default group is ReadOnly.  Cannot add addressable assets to it");
                return;
            }

            //var parentGroup = aaSettings.DefaultGroup;
			var createdEntry = aaSettings.CreateOrMoveEntry(targetGUID, parentGroup, false, false);

			aaSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryCreated, createdEntry, true, false);
        }
	}
}