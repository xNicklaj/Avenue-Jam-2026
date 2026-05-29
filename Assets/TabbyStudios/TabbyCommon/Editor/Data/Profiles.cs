using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public class Profiles
    {
        public static Profiles instance = new();
        
        public const string defaultProfile = "Default";
        
        private const string allProfilesKey = "__tabby_allProfiles";
        private string currentProfileKey => GetCurrentProfileKey();
        private const string nameIdSeparator = "_";

        public string currentProfile => EditorPrefs.GetString(currentProfileKey)?.RemoveAfterFirst(nameIdSeparator);
        
        private string projectGuid;

        private List<string> allProfileKeys => EditorPrefs.GetString(allProfilesKey).Split("__profile__").Where(s => !s.IsNullOrEmpty()).ToList();
        public List<string> currentProjectProfileNames => GetProjectProfileNames();

        public MenuDataSerializer menuSerializer;
        
        public Profiles(string projectId = null)
        {
            this.projectGuid = projectId.IsNullOrEmpty() ? PlayerSettings.productGUID.ToString() : projectId;
            Init();
        }

        public void Init()
        {
            var current = currentProfile;
            if (current.IsNullOrEmpty())
            {
                CreateProfile(defaultProfile);
                ChangeProfile(defaultProfile);
            }
            else if (!currentProjectProfileNames.Contains(current))
            {
                CreateProfile(current);
                ChangeProfile(current);
            }

            menuSerializer = new(this);
        }

        public bool ProfileExistsInThisProject(string name)
        {
            return ProfileExistsInProject(name, projectGuid);
        }

        public bool ProfileExistsInProject(string name, string projectId)
        {
            return GetProjectProfileNames(projectId).Contains(name);
        }

        public bool ProfileExistsInAnyProject(string name)
        {
            return allProfileKeys.Any(p => p.RemoveAfterLast(nameIdSeparator) == name);
        }
        
        public void ChangeProfile(string name)
        { 
            EditorPrefs.SetString(GetCurrentProfileKey(), GetProfileKey(name));
            menuSerializer?.OnProfilePathChanged();
        }

        public string CreateProfile(string name, DataNode tree = null)
        {
            name = GetUniqueName(name);
            var key = "__tabby_allProfiles";
            var allProfiles = EditorPrefs.GetString(key);
            EditorPrefs.SetString(key, $"{GetProfileKey(name)}{allProfiles}");

            tree ??= MenuBuilder.BuildTree();
            var items = tree.Flatten().Select(item => item.data).Where(data => !data.path.StartsWith(TabbyAssets.anonymousMenuPath)).ToList();
            string result = SafeJson.ToJson(new Profile(items, TabbyCommonFiles.version));
            SaveData(name, result);
            return name;
        }

        public string GetUniqueName(string name)
        {
            var pattern = @"\s*(\d+)?$";
            var asdf = Regex.Replace(name, pattern, "");
            var pattern2 = $"^{asdf}{pattern}";
            var matches = currentProjectProfileNames.Count(p => Regex.IsMatch(p, pattern2));
            if (matches == 0) return name;
            var uniqueName = $"{asdf} {matches + 1}";
            return uniqueName;
        }

        public void DeleteProfile(string name)
        {
            var key = GetProfileKey(name);
            EditorPrefs.SetString(allProfilesKey, EditorPrefs.GetString(allProfilesKey).Replace(key, ""));
            EditorPrefs.DeleteKey(key);
            if(currentProfile == name)
                ChangeProfile(defaultProfile);
        }

        public void DuplicateCurrentProfile(string newName)
        {
            CreateProfile(newName);
            instance.menuSerializer.DuplicateCurrentProfile(newName);
        }
        
        public string GetData(string profileName)
        {
            return EditorPrefs.GetString(GetProfileKey(profileName));
        }

        public void SaveData(string profileName, string data)
        {
            EditorPrefs.SetString(GetProfileKey(profileName), data);
        }

        private string GetProfileKey(string profileName)
        {
            return $"{profileName}_{projectGuid}__profile__";
        }

        private string GetCurrentProfileKey()
        {
            return $"__currentProfile_{projectGuid}";
        }

        private List<string> GetProjectProfileNames(string projectId = null)
        {
            var id = projectId.IfNullOrEmpty(projectGuid);
            return allProfileKeys.Where(p => p.EndsWith(id)).Select(p => p.RemoveTrailing(nameIdSeparator + id)).ToList();
        }

        public void Export(string profileName, string folder)
        {
            var path = $"{folder}/{profileName}.json";
            File.WriteAllText(path, GetData(profileName));
            Debug.Log($"Exported profile to {path}");
        }
        
        public void FullClear()
        {
            foreach (var name in allProfileKeys)
            {
                EditorPrefs.DeleteKey(name);
            }
            
            EditorPrefs.DeleteKey(currentProfileKey);
            EditorPrefs.DeleteKey(allProfilesKey);
        }

        public static void ClearProject(string projectId)
        {
            var profiles = new Profiles(projectId);
            EditorPrefs.DeleteKey(profiles.GetCurrentProfileKey());
            foreach (var profile in profiles.GetProjectProfileNames(projectId))
            {
                profiles.DeleteProfile(profile);
            }
        }
    }
}