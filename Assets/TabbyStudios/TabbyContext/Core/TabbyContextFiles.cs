using UnityEditor;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public class TabbyContextFiles : TabbyAssetData
    {
        public static string tabbyPath;
        public const string rootName = "TabbyContext";
        
        public static string documentationPath => $"{tabbyPath}/Manual.pdf";
        public static string examplesPath => $"{tabbyPath}/Data/Example Profiles";
        
        static TabbyContextFiles()
        {
            var current = TabbyFiles.ScriptDirUnityPath(nameof(TabbyContextFiles));
            tabbyPath = $"{current.RemoveAfterLast(rootName + "/")}{rootName}";
        }
    }
}