namespace Universe.Editor
{
	public class UPlatform
	{
		#region Constants
		
		public const string PLATFORM_DISPLAY_NAME_WIN64       = "Win64";
		public const string PLATFORM_EXTENSION_WIN64          = ".exe";
		public const string PLATFORM_DISPLAY_NAME_ANDROID  = "Android";
		public const string PLATFORM_EXTENSION_ANDROID     = ".apk";
		public const string PLATFORM_DISPLAY_NAME_PS5      = "PS5";
		public const string PLATFORM_EXTENSION_PS5         = "\\";

		public const string DEPLOY_SUFFIX = "Deploy";
		public const string DEPLOY_AND_RUN_SUFFIX = "DeployAndRun";
            
		public const string PS5_WORKSPACE_DEFAULT      = "workspace0";
		public const string PS5_WORKSPACE_DEVELOPMENT  = "workspace1";
		public const string PS5_WORKSPACE_RELEASE      = "workspace2";
		
		#endregion
	}
}