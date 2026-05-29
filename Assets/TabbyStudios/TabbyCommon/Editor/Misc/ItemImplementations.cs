using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TabbyStudios
{
    public static class ItemImplementations
    {
        public static void FindReferencesInScene()
        {
            var hierarchyType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var hierarchyWindow = EditorWindow.GetWindow(hierarchyType);
            var searchableEditorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SearchableEditorWindow");
            var setSearchFilterMethod = searchableEditorWindowType.GetMethod(
                "SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance, null, 
                new[] { typeof(string), typeof(SearchableEditorWindow.SearchMode), typeof(bool), typeof(bool) }, null);

            var reference = GetReference();
            if (reference.IsNullOrEmpty())
                return;
        
            if (setSearchFilterMethod != null)
            {
                setSearchFilterMethod.Invoke(hierarchyWindow, new object[] { reference, SearchableEditorWindow.SearchMode.All, true, false });
            }
            hierarchyWindow.Focus();
        }

        public static string GetReference()
        {
            var selectedObject = Selection.activeObject;
            if (selectedObject == null)
                return "";

            int instanceID = selectedObject.GetInstanceID();
            string reference = $"ref:{Math.Abs(instanceID)}:";
            return reference;
        }

        public static void SetDefaultParent()
        {
            var obj = Selection.activeGameObject;
            if (obj is not null)
                EditorUtility.SetDefaultParentObject(obj);
        }
    
        public static void ClearParent()
        {
            foreach (var obj in Selection.gameObjects)
            {
                if(Selection.gameObjects.Contains(obj.transform.parent.gameObject))
                    continue;
                obj.transform.parent = null;
            }
        }

        public static void UnpackPrefab()
        {
            Selection.gameObjects.Where(PrefabUtility.IsPartOfPrefabInstance).ForEach(obj => 
                PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction));
        }
    
        public static void UnpackPrefabCompletely()
        {
            Selection.gameObjects.Where(PrefabUtility.IsPartOfPrefabInstance).ForEach(obj => 
                PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.UserAction));    
        }

        private static void OpenAsset(PrefabStage.Mode mode)
        {
            if (Selection.activeGameObject is null) return;
            Object prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject);
            PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(prefabAsset), Selection.activeGameObject, mode); 
        }
    
        public static void OpenAssetInContext()
        {
            OpenAsset(PrefabStage.Mode.InContext);
        }

        public static void OpenAssetInIsolation()
        {
            OpenAsset(PrefabStage.Mode.InIsolation);
        }

        public static void SelectAsset()
        {
            Object prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject);
            EditorGUIUtility.PingObject(prefabAsset);
        }

        public static void SelectRoot()
        {
            Object[] roots = Selection.gameObjects.Select(PrefabUtility.GetOutermostPrefabInstanceRoot).ToArray();
            Selection.objects = roots;

        }
    
        private static GameObject targetInstance;
        private static bool keepOverrides;
        private const int pickerControlID = 123456;

        public static void Replace()
        {
            targetInstance = Selection.activeGameObject;
            keepOverrides = false;
            EditorUtil.DelayCall(1, () => EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:Prefab", pickerControlID));
            EditorUtil.DelayCall(2, OnObjectPickerUpdate);
        }

        public static void ReplaceAndKeepOverrides()
        {
            targetInstance = Selection.activeGameObject;
            keepOverrides = true;
            EditorUtil.DelayCall(1, () => EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:Prefab", pickerControlID));
            EditorUtil.DelayCall(2, OnObjectPickerUpdate);
        }

        private static void OnObjectPickerUpdate()
        {
            //todo this is not quite the same as the default
            GameObject selectedPrefab = EditorGUIUtility.GetObjectPickerObject() as GameObject;
            if (selectedPrefab != null && targetInstance != null)
            {
                Transform parent = targetInstance.transform.parent;
                Vector3 localPosition = targetInstance.transform.localPosition;
                Quaternion localRotation = targetInstance.transform.localRotation;
                Vector3 localScale = targetInstance.transform.localScale;
                GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                if (newInstance != null)
                {
                    newInstance.transform.SetParent(parent);
                    newInstance.transform.localPosition = localPosition;
                    newInstance.transform.localRotation = localRotation;
                    newInstance.transform.localScale = localScale;
                    if (keepOverrides)
                    {
                        var modifications = PrefabUtility.GetPropertyModifications(targetInstance);
                        if (modifications != null)
                        {
                            PrefabUtility.SetPropertyModifications(newInstance, modifications);
                        }
                    }

                    Undo.RegisterCreatedObjectUndo(newInstance, keepOverrides ? "Replace Prefab With Overrides" : "Replace Prefab");
                    Undo.DestroyObjectImmediate(targetInstance);
                    Selection.activeGameObject = newInstance;
                }
            
                targetInstance = null;
                return;
            }
        
            EditorApplication.delayCall += OnObjectPickerUpdate;
        }
        
        public static void CreateEmptyChild()
        {
            EditorApplication.ExecuteMenuItem("GameObject/Create Empty Child");
        }

        public static void ApplyToPrefab()
        {
            var prefab = EditorItemShowCondition.GetFarthestAncestorOrSelfPrefab(Selection.activeGameObject);
            if (prefab is null) return;
            
            var path = AssetDatabase.GetAssetPath(prefab);
            PrefabUtility.ApplyAddedGameObject(Selection.activeGameObject, path, InteractionMode.UserAction);
        }
    }
}