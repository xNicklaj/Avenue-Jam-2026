using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public class EditorItemShowCondition
    {
        private static Map<string, Func<bool>> defaultConditions;
        
        static EditorItemShowCondition()
        {
            var context = TabbyAssets.hasTabbyContext ? TabbyAssets.tabbyContextData.InvokeStaticMethod<Map<string, Func<bool>>>("GetDefaultShowConditions") : new();
            var menus = TabbyAssets.hasTabbyMenus ? TabbyAssets.tabbyMenusData.InvokeStaticMethod<Map<string, Func<bool>>>("GetDefaultShowConditions") : new();

            defaultConditions = Map<string, Func<bool>>.CombineMaps(context, menus);
        }

        public static Map<string, Func<bool>> showMap = new()
        {
            { nameof(AnythingSelected), AnythingSelected },
            { nameof(OneObjectSelected), OneObjectSelected },
            { nameof(PrefabSelected), PrefabSelected },
            { nameof(PartOfPrefabSelected), PartOfPrefabSelected },
            { nameof(OnePrefabIsSelected), OnePrefabIsSelected },
            { nameof(IsAddedGameObject), IsAddedGameObject },
        };

        public static bool ShouldShow(ItemData data)
        {
            if (showMap.ContainsKey(data.showWhen))
                return showMap[data.showWhen]();
            
            if (!defaultConditions.ContainsKey(data.originalPath))
                return true;

            return defaultConditions[data.originalPath]();
        }

        public static bool AnythingSelected()
        {
            var ids = Selection.instanceIDs;
            try
            {
                if (ProjectBrowserUtil.lastBrowser is null)
                {
                    return !ids.IsNullOrEmpty();
                }

                var left = ProjectBrowserUtil.lastBrowserLeftColumnRect.Contains(GUIUtility.GUIToScreenPoint(EditorInputHandler.input.current.mousePosition));
                var folderTree = ProjectBrowserUtil.lastBrowser.GetMemberValue("m_FolderTree");
                if (folderTree is null)
                    return !ids.IsNullOrEmpty();

                if (!left)
                {
                    var result = ids.None(id => folderTree.InvokeMethod<bool>("IsSelected", id));
                    return result;
                }
                else
                {
                    var leftIds = ProjectBrowserUtil.lastBrowser.GetMemberValue("m_FolderTree").InvokeMethod<int[]>("GetSelection");
                    return !leftIds.IsNullOrEmpty();
                }
            }
            catch
            {
                return !ids.IsNullOrEmpty();
            }
        }
    
        public static bool OneObjectSelected()
        {
            return AnythingSelected() && Selection.objects.Length == 1;
        }
    
        public static bool NoChildParent()
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length < 2)
                return false;
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                for (int j = i + 1; j < selectedObjects.Length; j++)
                {
                    GameObject obj1 = selectedObjects[i];
                    GameObject obj2 = selectedObjects[j];
                    if (IsParentOf(obj1.transform, obj2.transform) || 
                        IsParentOf(obj2.transform, obj1.transform))
                    {
                        return false;
                    }
                }
            }
        
            return true;
        }
    
        public static bool IsParentOf(Transform potentialParent, Transform child)
        {
            Transform current = child;
        
            while (current.parent != null)
            {
                if (current.parent == potentialParent)
                    return true;
                current = current.parent;
            }
        
            return false;
        }

        public static bool HasParent()
        {
            return AnythingSelected() && Selection.activeGameObject?.transform.parent is not null;
        }

        public static bool PrefabSelected()
        {
            return AnythingSelected() && Selection.gameObjects.Any(PrefabUtility.IsAnyPrefabInstanceRoot);
        }    
    
        public static bool PartOfPrefabSelected()
        {
            return AnythingSelected() && Selection.gameObjects.Any(PrefabUtility.IsPartOfPrefabInstance);
        }
    
        public static bool OnePrefabIsSelected()
        {
            return OneObjectSelected() && PrefabSelected();
        } 
    
        public static bool HasOnePartOfPrefab()
        {
            return OneObjectSelected() && PartOfPrefabSelected();
        }

        private static bool GameObjectsAreSelected()
        {
            return AnythingSelected() && Selection.activeGameObject is not null;
        }

        private static bool ObjectIsDescendedFromPrefab(GameObject obj)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(obj)) return true;
            var parent = obj?.transform?.parent?.gameObject;
            return parent is null ? false : ObjectIsDescendedFromPrefab(parent);
        }
        
        private static bool SelectionIsDescendedFromPrefab()
        {
            return GameObjectsAreSelected() && Selection.gameObjects.All(ObjectIsDescendedFromPrefab);
        }

        public static GameObject GetNearestAncestorOrSelfPrefab(GameObject obj)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(obj)) return obj;
            var parent = obj?.transform?.parent?.gameObject;
            return parent is null ? null : GetNearestAncestorOrSelfPrefab(parent);
        }
        
        public static GameObject GetFarthestAncestorOrSelfPrefab(GameObject obj, GameObject lastPrefab = null)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(obj))
                lastPrefab = obj;
            var parent = obj?.transform?.parent?.gameObject;
            return parent is null ? lastPrefab : GetFarthestAncestorOrSelfPrefab(parent, lastPrefab);
        }

        public static bool IsAddedGameObject()
        {
            if (Selection.gameObjects?.Length != 1 || !SelectionIsDescendedFromPrefab()) return false;

            List<GameObject> GetRootAddedObjects(GameObject obj)
            {
                return PrefabUtility.GetAddedGameObjects(GetNearestAncestorOrSelfPrefab(obj)).Select(o => o.instanceGameObject).ToList();
            }
            
            return GetRootAddedObjects(Selection.activeGameObject)?.Any(o => o == Selection.activeGameObject) ?? false;
        }
    
    }
}