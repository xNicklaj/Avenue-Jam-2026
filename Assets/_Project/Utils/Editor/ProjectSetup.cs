using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.VersionControl;
using UnityEngine;

using static System.Environment;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using Task = System.Threading.Tasks.Task;

public static class ProjectSetup {
    [MenuItem("Quick Actions/Setup/Toolsets/Install Inspector Tools")]
    public static void ImportInspectorTools() {        
        Assets.ImportAsset("vFavorites 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
        Assets.ImportAsset("vFolders 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
        Assets.ImportAsset("vHierarchy 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
        Assets.ImportAsset("vInspector 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
        Assets.ImportAsset("vTabs 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
        Assets.ImportAsset("Tabby Context - Enhanced Context Menus.unitypackage", "Tabby Studios/Editor ExtensionsUtilities", "Tabby Context");
        Assets.ImportAsset("Wingman - Your Inspectors Best Friend.unitypackage", "Kyle Rhoads/Editor ExtensionsUtilities", "Wingman");
        Assets.ImportAsset("Better Mesh - Mesh Preview Full-insight at a glance.unitypackage", "Tiny Giant Studio/Editor ExtensionsUtilities", "Better Mesh");
        Assets.ImportAsset("Better Transform - Size Notes Global-Local Workspace ParentChild Inspector.unitypackage", "Tiny Giant Studio/Editor ExtensionsUtilities", "Better Transform");
    }

    [MenuItem("Quick Actions/Setup/Toolsets/Install Development Tools")]
    public static void ImportDevTools()
    {
        Assets.ImportAsset("Editor Console Pro.unitypackage", "FlyingWorm/Editor ExtensionsSystem");
        Assets.ImportAsset("ALINE.unitypackage", "Aron Granberg/ScriptingGUI");
        Assets.ImportAsset("LogwinLog - Debug Values Tracker.unitypackage", "Julien Foucher/Editor ExtensionsUtilities", "Logwin");
        Assets.ImportAsset("Initargs.unitypackage", "Sisus/Editor ExtensionsUtilities", "Init(Args)");
        Assets.ImportAsset("Advanced FPS Counter.unitypackage", "Code Stage/Editor ExtensionsUtilities");
        Assets.ImportAsset("Binding System 2.unitypackage", "GGPostica/Editor ExtensionsUtilities");
        Assets.ImportAsset("PrimeTween High-Performance Animations and Sequences.unitypackage", "Kyrylo Kuzyk/Editor ExtensionsUtilities", "Prime Tween");
        Assets.ImportAsset("NaughtyAttributes.unitypackage", "Denis Rizov/Editor ExtensionsUtilities", "Prime Tween");
        Assets.ImportAsset("Scriptable Sheets.unitypackage", "Luna Wolf Studios LLC/Editor ExtensionsUtilities", "Prime Tween");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/Install Modeling Tools")]
    public static void ImportModelingTools()
    {
        Assets.ImportAsset("UModeler X.unitypackage", "UModeler Inc/Editor ExtensionsModeling");
        Assets.ImportAsset("RealBlend - Mesh Painting Creation.unitypackage", "Real Empire/Editor ExtensionsPainting", "RealBlend");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/Install Animation Tools")]
    public static void ImportAnimationTools()
    {
        Assets.ImportAsset("Animation Designer.unitypackage", "FImpossible Creations/Editor ExtensionsAnimation");
        Assets.ImportAsset("Look Animator.unitypackage", "FImpossible Creation/ScriptingAnimation");
        Assets.ImportAsset("Legs Animator.unitypackage", "FImpossible Creations/Editor ExtensionsAnimation");
        Assets.ImportAsset("Final IK.unitypackage", "RootMotion/Editor ExtensionsAnimation");
        Assets.ImportAsset("Animation Composer System - ACS.unitypackage", "Jorjouto/Editor ExtensionsAnimation", "Animation Composer System");
    }
    
    #region Shaders
    
    [MenuItem("Quick Actions/Setup/Toolsets/Lighting/Install Baked Lighting Tools")]
    public static void ImportBakedLightingTools()
    {
        Assets.ImportAsset("Bakery - GPU Lightmapper.unitypackage", "Mr F/Editor ExtensionsDesign");
        Assets.ImportAsset("Bakery Real-Time Preview.unitypackage", "Mr F/Editor ExtensionsDesign");
        Assets.ImportAsset("Magic Light Probes.unitypackage", "Eugene B/Editor ExtensionsUtilities");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/Lighting/Install Realtime Lighting Tools")]
    public static void ImportRealtimeLightingTools()
    {
        Assets.ImportAsset("HTrace Screen Space Global Illumination URP.unitypackage", "IPGames/Editor ExtensionsDesign", "HTrace SSGI");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/Shaders/Install AllInOne Shaders")]
    public static void ImportAIOShadersTools()
    {
        Assets.ImportAsset("All In 1 Sprite Shader.unitypackage", "Seaside Studios/Shaders");
        Assets.ImportAsset("All In 1 3D-Shader.unitypackage", "Seaside Studios/Shaders");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/Shaders/Install Custom Shaders Nodes")]
    public static void ImportCustomShadersTools()
    {
        Assets.ImportAsset("Seamless - Shader Graph Extension.unitypackage", "Lo Chaumartin/ScriptingEffects");
        Assets.ImportAsset("All In 1 Sprite Shader.unitypackage", "Seaside Studios/Shaders");
        Assets.ImportAsset("All In 1 3D-Shader.unitypackage", "Seaside Studios/Shaders");
        Assets.ImportAsset("All In 1 Vfx Toolkit.unitypackage", "Seaside Studios/VFX");
        Assets.ImportAsset("Lux URP Essentials.unitypackage", "forst/Shaders");
    }
    #endregion
    
    #region World Building
    [MenuItem("Quick Actions/Setup/Toolsets/World Building/Install Open World Tools")]
    public static void ImportHybridOpenWorldToolset()
    {
        Assets.ImportAsset("World Streamer 2.unitypackage", "NatureManufacture/Editor ExtensionsTerrain");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/World Building/Install Procedural World Building Tools")]
    public static void ImportProcgenWorldBuildingToolset()
    {
        Assets.ImportAsset("Procedural Terrain Generator - Vista Pro.unitypackage", "Pinwheel Studio/Editor ExtensionsTerrain", "Vista Pro");
        Assets.ImportAsset("Jupiter - Procedural Sky Shader Day Night Cycle.unitypackage", "Pinwheel Studio/Textures MaterialsSkies", "Jupiter");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/World Building/Install Hybrid World Building Tools")]
    public static void ImportHybridWorldBuildingToolset()
    {
        Assets.ImportAsset("RAM 3 - River Auto Material 3.unitypackage", "NatureManufacture/Editor ExtensionsTerrain", "River Auto Material 3");
        Assets.ImportAsset("Jupiter - Procedural Sky Shader Day Night Cycle.unitypackage", "Pinwheel Studio/Textures MaterialsSkies", "Jupiter");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/World Building/Install Handmade World Building Tools")]
    public static void ImportHanddrawnWorldBuildingToolset()
    {
        Assets.ImportAsset("Path Painter II.unitypackage", "3D Haven/Editor ExtensionsTerrain");
        Assets.ImportAsset("RAM 3 - River Auto Material 3.unitypackage", "NatureManufacture/Editor ExtensionsTerrain", "River Auto Material 3");
        Assets.ImportAsset("Jupiter - Procedural Sky Shader Day Night Cycle.unitypackage", "Pinwheel Studio/Textures MaterialsSkies", "Jupiter");
        Assets.ImportAsset("Prefab Brush.unitypackage", "Archie Andrews/Editor ExtensionsUtilities");
    }
    
    #endregion

    #region Optimization
    
    [MenuItem("Quick Actions/Setup/Toolsets/Optimization/Install Rendering Optimization Tools")]
    public static void ImportRenderingOptimizationTools()
    {
        Assets.ImportAsset("AutoLOD - Mesh Decimator.unitypackage", "Lo Chaumartin/Editor ExtensionsUtilities", "AutoLOD");
        Assets.ImportAsset("Mirage Pro - Runtime Impostors Baking System.unitypackage", "Lo Chaumartin/Editor ExtensionsUtilities", "Mirage Pro");
        Assets.ImportAsset("MeshFusion Pro Ultimate Optimization Tool.unitypackage", "New Game Studio/Editor ExtensionsUtilities", "MeshFusion Pro");
    }
    
    [MenuItem("Quick Actions/Setup/Toolsets/Optimization/Install Build Optimization Tools")]
    public static void ImportBuildOptimizationTools()
    {
        Assets.ImportAsset("Build Report Tool.unitypackage", "Anomalous Underdog/Editor ExtensionsUtilities");
        Assets.ImportAsset("Maintainer 2.unitypackage", "Code Stage/Editor ExtensionsUtilities");
    }
    
    #endregion

    // [MenuItem("Tools/Setup/Install Essential Packages")]
    // public static void InstallPackages() {
    //     Packages.InstallPackages(new[] {
    //         "com.unity.2d.animation",
    //         "git+https://github.com/adammyhre/Unity-Utils.git",
    //         "git+https://github.com/adammyhre/Unity-Improved-Timers.git",
    //         "git+https://github.com/KyleBanks/scene-ref-attribute.git"
    //         // If necessary, import new Input System last as it requires a Unity Editor restart
    //         // "com.unity.inputsystem"
    //     });
    // }

    [MenuItem("Quick Actions/Setup/Setup Folders")]
    public static void CreateFolders() {
        Folders.Create("_Project");
        Refresh();
        Folders.Move("_Project", "Scenes");
        Folders.Move("_Project", "Settings");
        Folders.Delete("TutorialInfo");
        Refresh();

        MoveAsset("Assets/InputSystem_Actions.inputactions", "Assets/_Project/Settings/InputSystem_Actions.inputactions");
        DeleteAsset("Assets/Readme.asset");
        Refresh();
        
        // Optional: Disable Domain Reload
        EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
    }

    static class Assets {
        public static void ImportAsset(string asset, string folder, [CanBeNull] string name = null) {
            string basePath;
            string viewname = name ?? asset.Split(".")[0];
            
            if (OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix) {
                string homeDirectory = GetFolderPath(SpecialFolder.Personal);
                basePath = Combine(homeDirectory, "Library/Unity/Asset Store-5.x");
            } else {
                string defaultPath = Combine(GetFolderPath(SpecialFolder.ApplicationData), "Unity");
                basePath = Combine(EditorPrefs.GetString("AssetStoreCacheRootPath", defaultPath), "Asset Store-5.x");
            }

            asset = asset.EndsWith(".unitypackage") ? asset : asset + ".unitypackage";

            string fullPath = Combine(basePath, folder, asset);

            if (!File.Exists(fullPath)) {
                Debug.LogError($"The asset {viewname} package was not found at the path: {fullPath}");
                return;
            }
            
            ImportPackage(fullPath, false);
            Debug.Log($"Asset {viewname} has been installed!");
        }
    }

    static class Packages {
        static AddRequest request;
        static Queue<string> packagesToInstall = new Queue<string>();

        public static void InstallPackages(string[] packages) {
            foreach (var package in packages) {
                packagesToInstall.Enqueue(package);
            }

            if (packagesToInstall.Count > 0) {
                StartNextPackageInstallation();
            }
        }

        static async void StartNextPackageInstallation() {
            request = Client.Add(packagesToInstall.Dequeue());
            
            while (!request.IsCompleted) await Task.Delay(10);
            
            if (request.Status == StatusCode.Success) Debug.Log("Installed: " + request.Result.packageId);
            else if (request.Status >= StatusCode.Failure) Debug.LogError(request.Error.message);

            if (packagesToInstall.Count > 0) {
                await Task.Delay(1000);
                StartNextPackageInstallation();
            }
        }
    }

    static class Folders {
        public static void Create(string root, params string[] folders) {
            var fullpath = Combine(Application.dataPath, root);
            if (!Directory.Exists(fullpath)) {
                Directory.CreateDirectory(fullpath);
            }

            foreach (var folder in folders) {
                CreateSubFolders(fullpath, folder);
            }
        }
        
        static void CreateSubFolders(string rootPath, string folderHierarchy) {
            var folders = folderHierarchy.Split('/');
            var currentPath = rootPath;

            foreach (var folder in folders) {
                currentPath = Combine(currentPath, folder);
                if (!Directory.Exists(currentPath)) {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }
        
        public static void Move(string newParent, string folderName) {
            var sourcePath = $"Assets/{folderName}";
            if (IsValidFolder(sourcePath)) {
                var destinationPath = $"Assets/{newParent}/{folderName}";
                var error = MoveAsset(sourcePath, destinationPath);

                if (!string.IsNullOrEmpty(error)) {
                    Debug.LogError($"Failed to move {folderName}: {error}");
                }
            }
        }
        
        public static void Delete(string folderName) {
            var pathToDelete = $"Assets/{folderName}";

            if (IsValidFolder(pathToDelete)) {
                DeleteAsset(pathToDelete);
            }
        }
    }
}