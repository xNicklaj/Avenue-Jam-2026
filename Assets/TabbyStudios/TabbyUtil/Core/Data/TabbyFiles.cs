using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TabbyStudios
{
    public static class TabbyFiles
    {
        public static string separator = "/";
        public static string rootName = "TabbyUtil";
        public static string tabbyPath;

        public static string configPath => $"{tabbyPath}/Core/Data/Config/config.json";
        public static string newConfigPath => $"{tabbyPath}/Core/Data/NewConfig/config.json";
        
        static TabbyFiles()
        {
            var current = ScriptDirUnityPath(nameof(TabbyFiles));
            tabbyPath = $"{current.RemoveAfterLast(rootName+separator)}{rootName}";
        }
        
        public static string ScriptDirUnityPath(string scriptName)
        {

            #if  UNITY_EDITOR
            var g = AssetDatabase.FindAssets($"t:Script {scriptName}");
            var editorPath = AssetDatabase.GUIDToAssetPath(g[0]);
            return editorPath.FixSlashes();
            #else
            var scriptFilePaths = Directory.GetFiles(Application.dataPath, $"{scriptName}.cs", SearchOption.AllDirectories);
            if (scriptFilePaths.Length == 0)
                throw new FileNotFoundException($"Script '{scriptName}' not found in project");
            var currentPath = scriptFilePaths[0].Replace(Application.dataPath, "Assets").FixSlashes();
            return currentPath.FixSlashes();
            #endif
        }
        
        public static void CreateFolder(string path)
        {
            #if UNITY_EDITOR
            var steps = path.Step(separator, includeLast: true).Skip(1);
            foreach (var step in steps)
            {
                if (!AssetDatabase.IsValidFolder(step))
                {
                    CreateSingleFolder(step);
                }
            }
            #else
            string fullPath = GetFullPath(path);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            #endif
        }
    
        public static void DeleteFolder(string path)
        {
            Assert.AreNotEqual("Assets", path.RemoveTrailing(separator));
            #if UNITY_EDITOR
            AssetDatabase.DeleteAsset(path);
            #else
            string fullPath = GetFullPath(path);
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                File.Delete($"{fullPath}.meta");
            }
            #endif
        }
        
        public static void DeleteFile(string path)
        {
            Assert.AreNotEqual("Assets", path.RemoveTrailing(separator));
            File.Delete(path);
        }
        
        public static void CreateSingleFolder(string path)
        {
            #if UNITY_EDITOR
            AssetDatabase.CreateFolder(path.RemoveAfterLast(separator), path.RemoveBeforeLast(separator));
            #endif
        }
        
        public static string ArbitraryNewFolderPath(string parentPath)
        {
            string current = "a";
            for (int i = 0; i < 20; i++)
            {
                var path = CreatePath(parentPath, current);
                string fullPath = GetFullPath(path);
                if (!Directory.Exists(fullPath))
                    return path;

                current += "a";
            }

            throw new Exception("Couldn't create new folder name");
        }

        public static string CreatePath(string parentPath, string name)
        {
            return $"{parentPath}{separator}{name}";
        }
    
    
        public static string SafeRead(string path, string _default = "")
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(FolderPath(path));
                File.WriteAllText(path,_default);
                return _default;
            }

            return File.ReadAllText(path);
        }
        
        public static void SafeWrite(string path, string content)
        {
            Directory.CreateDirectory(FolderPath(path));
            File.WriteAllText(path, content);
        }
        
        public static string FolderPath(string filePath)
        {
            return filePath.RemoveAfterLast("/");
        }
        
        private static string GetFullPath(string unityPath)
        {
            if (unityPath.StartsWith("Assets"))
            {
                return Path.Combine(Application.dataPath, unityPath.Substring(7)).FixSlashes();
            }
            return unityPath;
        }
        
        private static string FixSlashes(this string s)
        {
            return s.ReplaceMultiple("\\", "/");
        }

        public static string GetFileNameWithoutExtension(string s)
        {
            return s.FixSlashes().RemoveBeforeLast("/").RemoveAfterLast(".");
        }
        
        public static string GetFileNameWithExtension(string s)
        {
            return s.FixSlashes().RemoveBeforeLast("/");
        }

        public static List<string> GetFilesExcludeMeta(string folder)
        {
            return Directory.GetFiles(folder).Where(file => !file.EndsWith(".meta")).ToList();
        }
        
        public static List<string> SafeGetFiles(string folder, string pattern)
        {
            if(!Directory.Exists(folder))
                CreateFolder(folder);

            return Directory.GetFiles(folder, pattern).ToList();
        }
    }
}