using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class HasChildrenToggle : VisualComponent, ItemDataModifier
    {
        public ItemData data { get; set; }

        public override void Start()
        {
            var toggle = target as Toggle;
            toggle.value = data.hasChildren;
            toggle.RegisterCallback<ChangeEvent<bool>>(ChangeChildren);
        }

        private void ChangeChildren(ChangeEvent<bool> e)
        {
            Profiles.instance.menuSerializer.Insert($"{data.path}/TempItem");
        }

    }
    
    public class UseGradientToggle : VisualComponent, ItemDataModifier
    {
        public ItemData data { get; set; }

        public override void Start()
        {
            var toggle = target as Toggle;
            toggle.value = data.useGradient;
            toggle.RegisterCallback<ChangeEvent<bool>>(ChangeGradient);
        }

        private void ChangeGradient(ChangeEvent<bool> e)
        {
            Profiles.instance.menuSerializer.SetProperty(data, i => i.useGradient = !data.useGradient);
            SettingsMenuManager.StaticRefresh();
        }
    }

    public class FallbackInputHandlingToggle : SerializedSetting<bool>
    {
        public override string setting => "fallbackInputHandling";

        public override void OnChange(ChangeEvent<bool> e)
        {
            base.OnChange(e);
            SetAssemblyPlatform("Assets/TabbyStudios/TabbyContext/Patching/TabbyContextPatching.asmdef", !e.newValue);
            SetAssemblyPlatform("Assets/TabbyStudios/TabbyUtil/Core/Patching/TabbyUtilPatching.asmdef", !e.newValue);
            SetDLL(!e.newValue);
            CompilationPipeline.RequestScriptCompilation();
        }

        private void SetAssemblyPlatform(string path, bool include)
        {
            string json = File.ReadAllText(path);
            var asmdefData = JsonUtility.FromJson<AsmdefData>(json);
            if (include)
            {
                asmdefData.includePlatforms = new []{"Editor"};
                asmdefData.excludePlatforms = new string[] {};
            }
            else
            {
                asmdefData.excludePlatforms = new []{"Editor"};
                asmdefData.includePlatforms = new string[] {};
            }
            
            string modifiedJson = JsonUtility.ToJson(asmdefData, true);
            File.WriteAllText(path, modifiedJson);
            AssetDatabase.Refresh();
        }
        
        public static void SetDLL(bool include)
        {
            string dllPath = "Assets/TabbyStudios/Harmony/net48/0Harmony.dll";
            var importer = (PluginImporter)AssetImporter.GetAtPath(dllPath);
            importer.SetCompatibleWithEditor(include);
            importer.SetCompatibleWithAnyPlatform(false);
            importer.SaveAndReimport();
        }

        [System.Serializable]
        private class AsmdefData
        {
            public string name;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public string[] precompiledReferences;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public bool autoReferenced;
            public bool noEngineReferences;
        }
    }

}