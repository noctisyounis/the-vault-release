using UnityEditor;

namespace Symlink.Editor
{
    public static class SymlinkMenuItem
    {
        [MenuItem("Assets/Create/Symlink Folder", false, 20)]
        private static void SymlinkAbsolute()
        {
            SymlinkEditor.Symlink();
        }
    }
}