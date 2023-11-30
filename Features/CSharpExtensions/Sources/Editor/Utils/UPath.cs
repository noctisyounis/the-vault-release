using static UnityEngine.Application;

namespace Universe.Editor
{
	public class UPath
	{
		#region Constants

		//Absolute paths
		public const string STEAM_CONTENT_BUILDER_PATH = "D:\\steamworks_sdk_153a\\sdk\\tools\\ContentBuilder\\content";
		public const string KEYSTORE_PATH			   = "C:\\Keystores\\KeystoreInfo.txt";

		//Project relative
		public const string TASK_GAME_STARTER_PATH     = "Assets\\_\\GameStarter\\GameStarter.unity";
		public const string BUILD_PATH                 = "..\\Builds";
		public const string VERSION_PATH			   = "..\\CommitInfo.txt";
		public const string LOCAL_COMMIT_ID_PATH       = "..\\JenkinsUtility\\Common\\WorkspaceCommitID.txt";
		public const string BUILD_SLACK_MOVER_PATH     = "..\\JenkinsUtility\\Common\\Jenkins_Slack_Uploader.bat";
		public const string BUILD_STEAM_MOVER_PATH     = "..\\JenkinsUtility\\Win64\\Jenkins_Steam_Mover.bat";
		public const string ANDROID_TEMPORARY_MANIFEST = "..\\Temp\\StagingArea\\AndroidManifest.xml";
		public const string ANDROID_CUSTOM_MANIFEST	   = "Plugins\\Android\\AndroidManifest.xml";
			

		//.bat relative
		public const string UPLOAD_PATH = "..\\Builds";

		//static names
		public const string DEVELOPMENT_BUILD_PREFIX       = "[DEV]";
		public const string RELEASE_BUILD_PREFIX           = "[Release]";
		public const string DO_NOT_SHIP_BURST_SUFFIX       = "_BurstDebugInformation_DoNotShip";
		public const string DO_NOT_SHIP_IL_SUFFIX          = "_BackUpThisFolder_ButDontShipItWithYourGame";
		
		public static string SourceDirectoryPath => $"{dataPath}\\..\\..\\Symlinks";
		public static string SourceGraphicsTiersDirectoryPath => $"{SourceDirectoryPath}\\GraphicsTier";
		public static string TargetDirectoryPath => $"{dataPath}\\_\\Content";
		public static string TargetGraphicsTiersDirectoryPath => $"{TargetDirectoryPath}\\GraphicsTier";
		
		#endregion
	}
}
