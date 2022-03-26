using UnityEngine;

using static System.IO.Directory;
using static UnityEngine.Debug;
using static UnityEngine.Application;

namespace Universe
{
    public static class UFile
    {
        #region Public

        public static string UGetOrCreateFolderAt( string path )
        {
            CreateDirectoryIfNotExist( path );

            return $"{path}/";
        }

        public static string UGetPathRelativeToProject( string folderPath )
        {
            if(string.IsNullOrEmpty(folderPath))
            {
                LogError( $"ERROR path is null or empty." );
                return null;
            }

            if( IsNotSystemPath( folderPath ) ) return folderPath;
            
            if( IsNotInProject( folderPath ) )
            {
                LogError( $"ERROR path {folderPath} is not in Project folder and cannot be converted into path relative to it." );
                return null;
            }

            return RemoveSystemPathFrom( folderPath );
        }

        public static string URemoveAssetsPathFrom( string folderPath ) => 
            $"{dataPath.Remove( dataPath.Length - 6, 6 )}{folderPath}";

        #endregion


        #region Utilities

        private static void CreateDirectoryIfNotExist( string folderPath )
        {
            if( IsNotSystemPath( folderPath ) ) folderPath = URemoveAssetsPathFrom( folderPath );

            if( Exists( folderPath ) ) return;

            CreateDirectory( folderPath );
        }

        private static string RemoveSystemPathFrom( string inProjectPath )
        {
            var assetsIndex = inProjectPath.IndexOf( ASSETS_DIRECTORY );
            return inProjectPath.Substring( assetsIndex );
        }

        private static bool IsNotSystemPath( string folderPath ) => folderPath[1] != ':' && ( folderPath[2] != '/' || folderPath[2] != '\\' );
        private static bool IsNotInProject( string folderPath ) => !folderPath.Contains( $"/{ASSETS_DIRECTORY}" );

        private const string ASSETS_DIRECTORY = "Assets/";

        #endregion
    }
}