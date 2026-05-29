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
        private static Map<string, bool> cachedResults = new();
        
        private static List<IdWrapper> selection = new();
        private static List<IdWrapper> leftColumnSelection = new();
        private static List<IdWrapper> correctSelection => left ? leftColumnSelection : selection;
        private static bool left;

        public static Map<string, Func<bool>> showMap = new()
        {
            { nameof(AnythingSelected), AnythingSelected },
            { nameof(FolderSelected), FolderSelected },
            { nameof(NotFolderSelected), NotFolderSelected },
            { nameof(OneObjectSelected), OneObjectSelected },
            { nameof(PrefabSelected), PrefabSelected },
            { nameof(PartOfPrefabSelected), PartOfPrefabSelected },
            { nameof(OnePrefabIsSelected), OnePrefabIsSelected },
            { nameof(IsAddedGameObject), IsAddedGameObject },
            { nameof(GameObjectsAreSelected), GameObjectsAreSelected },
            { nameof(HasClipboard), HasClipboard },
            { nameof(CanPasteSpecial), CanPasteSpecial },
        };
        
        static EditorItemShowCondition()
        {
            var context = TabbyAssets.hasTabbyContext ? TabbyAssets.tabbyContextData.InvokeStaticMethod<Map<string, Func<bool>>>("GetDefaultShowConditions") : new();
            var menus = TabbyAssets.hasTabbyMenus ? TabbyAssets.tabbyMenusData.InvokeStaticMethod<Map<string, Func<bool>>>("GetDefaultShowConditions") : new();

            defaultConditions = Map<string, Func<bool>>.CombineMaps(context, menus);
        }
        
        public static bool ShouldShow(ItemData data)
        {
            var condition = defaultConditions.TryGetValue(data.originalPath, out var func) ? func.Method.Name : data.showWhen;

            if (cachedResults.TryGetValue(condition, out var result))
            {
                return result;
            }
            if (showMap.TryGetValue(condition, out func))
            {
                return cachedResults[condition] = func();
            }

            return true;
        }

        public static void RefreshSelection()
        {
            cachedResults = new();
            selection = IdWrapper.CreateFromSelection();
            
            left = ProjectBrowserUtil.IsTwoColumns() && ProjectBrowserUtil.lastBrowserLeftColumnRect.Contains(GUIUtility.GUIToScreenPoint(EditorInputHandler.input.current.mousePosition));
            if (!left) return;
            
            if (ProjectBrowserUtil.lastBrowser is null) return;

            var folderTree = ProjectBrowserUtil.lastBrowser.GetMemberValue("m_FolderTree");
            if (folderTree is null) return;

            leftColumnSelection = IdWrapper.Create(((Array)folderTree.InvokeMethod("GetSelection")).Cast<object>().ToArray());
        }

        public static bool FolderSelected()
        {
            return correctSelection.Any(id => AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(id)));
        }

        public static bool NotFolderSelected()
        {
            return !FolderSelected() && AnythingSelected();
        }

        public static bool AnythingSelected()
        {
            #if UNITY_6000_3_OR_NEWER
            var r =  NewAnythingSelected();
            return r;
            #else
            return OldAnythingSelected();
            #endif
        }
        
        public static bool NewAnythingSelected()
        {
            #if UNITY_6000_3_OR_NEWER
            if (left)
            {
                return !leftColumnSelection.IsNullOrEmpty();
            }
            else
            {
                return !selection.IsNullOrEmpty();
            }
            
            #endif
            #pragma warning disable CS0162 // Unreachable code detected
            // ReSharper disable once HeuristicUnreachableCode
            return false;
            #pragma warning restore CS0162 // Unreachable code detected
        }


        public static bool OldAnythingSelected()
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            var ids = Selection.instanceIDs;
            #pragma warning restore CS0618 // Type or member is obsolete
            try
            {
                if (ids.IsNullOrEmpty()) return false;
                if (ProjectBrowserUtil.lastBrowser is null) return true;

                var left = ProjectBrowserUtil.lastBrowserLeftColumnRect.Contains(GUIUtility.GUIToScreenPoint(EditorInputHandler.input.current.mousePosition));
                var folderTree = ProjectBrowserUtil.lastBrowser.GetMemberValue("m_FolderTree");
                if (folderTree is null) return true;
                
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
            
            #pragma warning disable CS0162 // Unreachable code detected
            // ReSharper disable once HeuristicUnreachableCode
            return false;
            #pragma warning restore CS0162 // Unreachable code detected
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
            return GameObjectsAreSelected() && Selection.activeGameObject?.transform.parent is not null;
        }

        public static bool PrefabSelected()
        {
            return GameObjectsAreSelected() && Selection.gameObjects.Any(PrefabUtility.IsAnyPrefabInstanceRoot);
        }    
    
        public static bool PartOfPrefabSelected()
        {
            return GameObjectsAreSelected() && Selection.gameObjects.Any(PrefabUtility.IsPartOfPrefabInstance);
        }
    
        public static bool OnePrefabIsSelected()
        {
            return OneObjectSelected() && PrefabSelected();
        } 
    
        public static bool HasOnePartOfPrefab()
        {
            return OneObjectSelected() && PartOfPrefabSelected();
        }

        public static bool GameObjectsAreSelected()
        {
            return Selection.activeGameObject is not null;
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

        public static bool HasClipboard()
        {
            return typeof(Unsupported).InvokeStaticMethod<bool>("CanPasteGameObjectsFromPasteboard");
        }

        public static bool CanPasteSpecial()
        {
            return HasClipboard() && GameObjectsAreSelected();
        }

        #if UNITY_6000_3_OR_NEWER
        private static void LogSelectionPaths()
        {
            var leftNames = leftColumnSelection.Select(id =>
            {
                var obj = EditorUtility.EntityIdToObject(id);
                if (obj == null)
                {
                    return "Selection has null obj";
                }

                string path = AssetDatabase.GetAssetPath(obj);
                return path.IsNullOrEmpty() ? obj.name : path;
            }).ToList();
            if (!leftNames.IsNullOrEmpty())
            {
                Debug.Log("Left");
                leftNames.Log();
            }
            
            var rightNames = selection.Select(id =>
            {
                var obj = EditorUtility.EntityIdToObject(id);
                if (obj == null)
                {
                    return "Selection has null obj";
                }

                string path = AssetDatabase.GetAssetPath(obj);
                return path.IsNullOrEmpty() ? obj.name : path;
            }).ToList();

            if (!rightNames.IsNullOrEmpty())
            {
                Debug.Log("Right");
                rightNames.Log();
            }
        }
        #endif
    }
}