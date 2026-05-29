using System;
using System.Collections.Generic;
using UnityEditor;

namespace TabbyStudios
{
    public class ItemRunner
    {
        private static Dictionary<string,Action> strangeItems = new Dictionary<string,Action>()
        {
            {"GameObject/Find References In Scene", ItemImplementations.FindReferencesInScene},
            {"GameObject/Set As Default Parent", ItemImplementations.SetDefaultParent},
            {"GameObject/Clear Parent", ItemImplementations.ClearParent},
            {"GameObject/Prefab/Unpack", ItemImplementations.UnpackPrefab},
            {"GameObject/Prefab/Unpack Completely",ItemImplementations.UnpackPrefabCompletely },
            {"GameObject/Prefab/Open Asset In Context", ItemImplementations.OpenAssetInContext},
            {"GameObject/Prefab/Open Asset In Isolation", ItemImplementations.OpenAssetInIsolation},
            {"GameObject/Prefab/Select Asset", ItemImplementations.SelectAsset},
            {"GameObject/Prefab/Select Root", ItemImplementations.SelectRoot},
            {"GameObject/Prefab/Replace", ItemImplementations.Replace},
            {"GameObject/Prefab/Replace And Keep Overrides", ItemImplementations.ReplaceAndKeepOverrides},
            {"GameObject/Create Empty", ItemImplementations.CreateEmptyChild},
            {"GameObject/Added GameObject/Apply to Prefab", ItemImplementations.ApplyToPrefab},
        };
    
        public static bool Run(string path)
        {
            if (Config.GetSetting<bool>("fallbackInputHandling"))
            {
                if (RunWithoutStoredCall(path)) return true;
            }
            else
            {
                if (RunWithStoredCall(path)) return true;
            }

            return RunCommon(path);
        }

        private static bool RunWithStoredCall(string path)
        {
            if (MenuCalls.TryCall(path))
                return true;
            
            if (path.StartsWith("Edit/"))
            {
                RunEdit(path);
                return true;
            }
            
            if(!path.IsNullOrEmpty())
            {
                EditorApplication.ExecuteMenuItem(path);
                return true;
            }

            return false;
        }

        private static bool RunWithoutStoredCall(string path)
        {
            path = TabbyAssets.MapToUnityPath(path);
            
            if (strangeItems.ContainsKey(path))
            {
                strangeItems[path]();
                return true;
            }
            return false;
        }

        private static bool RunCommon(string path)
        {
            if (path.StartsWith("Edit/"))
            {
                RunEdit(path);
                return true;
            }
                
            if(!path.IsNullOrEmpty())
            {
                EditorApplication.ExecuteMenuItem(path);
                return true;
            }

            return false;
        }

        public static void RunEdit(string path)
        {
            var objs = Selection.objects;
            Selection.objects = null;
            EditorApplication.delayCall += () =>
            {
                Selection.objects = objs;
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.ExecuteMenuItem(path);
                };
            };
        }
    }
}