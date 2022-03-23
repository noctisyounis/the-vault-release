using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;

using static UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject;
using static UnityEditor.AssetDatabase;
using static Universe.UAddressableUtility;

namespace Universe.Editor
{
	public static class UGroupHelper
	{
		
		#region Public

		public static string s_targetFolder = "Assets/_/Content";
		public static Action OnRefreshCompleted;

		#endregion


		#region Main

		[MenuItem("Vault/Adressable/Refresh groups")]
		public static void RefreshAaGroups()
		{
			Debug.Log("Refresh started");
			RefreshPaths();

			s_currentFolder = s_folderPaths.Count - 1;
			RefreshFolderAt(s_currentFolder);
		}

		#endregion


		#region Utils 

		private static void ScanFolders()
		{
			s_folderPaths = new List<string>();

			var folders = GetSubFolders(s_targetFolder);

			foreach (var folder in folders)
			{
				ScanFoldersRecursive(folder);
			}
		}

		private static void ScanFoldersRecursive(string folder)
		{
			s_folderPaths.Add(folder);

			var folders = GetSubFolders(folder);

			foreach (var fld in folders)
			{
				ScanFoldersRecursive(fld);
			}
		}

		private static void RefreshPaths()
		{
			var allPaths = GetAllAssetPaths();

			ScanFolders();

			s_assetPaths = new List<string>();
			s_helperPaths = new List<string>();
            
            foreach (var path in allPaths)
            {
                if(!path.Contains(s_targetFolder)) continue;
                if(IsValidFolder(path)) continue;

				if(GetMainAssetTypeAtPath(path).Equals(typeof(UAddressableGroupHelper)))
				{
					s_helperPaths.Add(path);
					continue;
				}

				s_assetPaths.Add(path);
            }
		}

		private static void RefreshFolderAt(int index)
		{
			if(index < 0 || index >= s_folderPaths.Count) 
			{
				Debug.LogWarning("[ADDRESSABLE REFRESH] No existing folder to refresh");
				return;
			}

			var folderPath = s_folderPaths[index];

			for(var i = s_helperPaths.Count - 1; i >= 0; i--)
			{
				var path = s_helperPaths[i];

				if(!path.Contains(folderPath)) continue;
				
				var item = LoadMainAssetAtPath(path);
				var helper = (UAddressableGroupHelper)item;
				
				if(!helper.m_group)
				{
					helper.GenerateNewGroup();
				}

				s_waitedHelper = helper;
				s_helperPaths.Remove(path);

				RefreshEntriesIn(folderPath, s_waitedHelper.m_group);
				return;
			}

			s_currentFolder--;

			if(s_currentFolder < 0) 
			{
				FinalizeRefresh();
				return;
			}

			RefreshFolderAt(s_currentFolder);
		}

		private static void RefreshEntriesIn(string folderPath, AddressableAssetGroup parentGroup)
		{
			for(var i = s_assetPaths.Count - 1; i >= 0; i--)
			{
				var path = s_assetPaths[i];

				if(!path.Contains(folderPath)) continue;

				s_assetPaths.Remove(path);
				var guid = GUIDFromAssetPath(path).ToString();

				RemoveAaEntry(Settings, guid);
				CreateAaEntry(Settings, guid, parentGroup);
			}

			s_currentFolder--;

			if(s_currentFolder >= 0)
			{
				RefreshFolderAt(s_currentFolder);
				return;
			}

			FinalizeRefresh();
		}

		private static void FinalizeRefresh()
		{
			USettings.GetSettings<UGraphicsSettings>().GetPathTable().Populate();
			OnRefreshCompleted?.Invoke();
		}

		#endregion


		#region Private Members

		private static List<string> s_folderPaths;
		private static List<string> s_assetPaths;
		private static List<string> s_helperPaths;
		private static UAddressableGroupHelper s_waitedHelper;
		private static int s_currentFolder;

		#endregion
    }
}