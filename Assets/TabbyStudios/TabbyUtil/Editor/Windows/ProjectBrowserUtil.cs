using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public class ProjectBrowserUtil
    {
        private static EditorWindow _lastBrowser;
        public static EditorWindow lastBrowser
        {
            get => _lastBrowser ??= UnityWindows.projectBrowser;
            set => _lastBrowser = value;
        }

        public static Rect lastBrowserLeftColumnRect => new Rect(lastBrowser.position.position, LeftColumnSize());
        
        public static bool doubleColumn
        {
            get => lastBrowser.GetFieldValue<int>("m_ViewMode") == 1;
            set => lastBrowser.InvokeMethod("InitViewMode", value ? 1 : 0);
        }
        
        public const float barHeight = 25;
        public const float topBarHeight = 44;
        public const float scrollBarWidth = 14;
        public const float bottomBarHeight = 20;
        public const float windowResizeMargin = 6;
        
        static ProjectBrowserUtil()
        {
            UnityWindows.onFocusChanged += TrackFocusedWindow;
        }
    
        public static void TrackFocusedWindow()
        {
            var focusedWindow = EditorWindow.focusedWindow;
            if (focusedWindow?.GetType().Name == "ProjectBrowser")
            {
                lastBrowser = focusedWindow;
            }
        }
        
        public static Vector2 Size()
        {
            return lastBrowser.position.size;
        }
    
        public static Vector2 LeftColumnSize()
        {
            if (!doubleColumn)
                return new Vector2(0, 0);
            var width = lastBrowser.GetFieldValue<float>("m_DirectoriesAreaWidth");
            var height = Size().y;
            return new Vector2(width, height);
        }
    
        public static float LeftColumnWidth()
        {
            return LeftColumnSize().x;
        }
    
        public static float LeftColumnHeight()
        {
            return LeftColumnSize().y;
        }
    
        public static Vector2 RightColumnSize()
        {
            return Size().SubX(LeftColumnSize().x);
        }
    
        public static float RightColumnWidth()
        {
            return RightColumnSize().x;
        }
    
        public static float RightColumnHeight()
        {
            return RightColumnSize().y;
        }
        
        private static object LoadLocalAssets()
        {
            var listArea = lastBrowser.GetFieldValue("m_ListArea");
            return listArea.GetFieldValue("m_LocalAssets");
        }
        
        private static bool IsInRightColumn(string guid, Dictionary<string, int> idMap)
        {
            return idMap.ContainsKey(guid);
        }
        
        public static Rect ConvertItemRect(Rect r)
        {
            var offset = new Vector2(LeftColumnWidth()+30, 80) + lastBrowser.position.position;
            return new Rect(r.position + offset, r.size);
        }

        public static string GetOpenFolder()
        {
            return UnityWindows.projectBrowser?.InvokeMethod<string>("GetActiveFolderPath");
        }

        public static void SetOpenFolder(string path)
        {
            #if UNITY_6000_3_OR_NEWER
            var id = AssetDatabase.LoadAssetAtPath<Object>(path).GetEntityId();
            UnityWindows.projectBrowser?.InvokeMethod("ShowFolderContents", id, true); 
            #else
            int id = AssetDatabase.LoadAssetAtPath<Object>(path).GetInstanceID();
            UnityWindows.projectBrowser?.InvokeMethod("ShowFolderContents", id, true); 
            #endif

            Assert.AreEqual(path, GetOpenFolder());
        }

        public static void SelectFolderInLeftColumn(string path)
        {
            SetOpenFolder(path);
            #if UNITY_6000_3_OR_NEWER
            Assert.Contains(path, Selection.entityIds.Select(id =>
            {
                Object obj = EditorUtility.EntityIdToObject(id);
                string path = AssetDatabase.GetAssetPath(obj);
                return path;
            }).ToList());
            #endif
        }

        public static void SetToOneColumn()
        {
            UnityWindows.projectBrowser.InvokeMethod("SetViewMode", 0);   
        }

        public static void SetToTwoColumns()
        {
            UnityWindows.projectBrowser.InvokeMethod("SetViewMode", 1);   
        }

        public static bool IsTwoColumns()
        {
            return UnityWindows.projectBrowser?.InvokeMethod<bool>("IsTwoColumns") ?? false;
        }
        
        //be careful these are extremely slow they're mostly for testing
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        private class GUIWrapper
        {
            private object localAssets;
            private Dictionary<string, int> idMap;
            private List<(string, Rect)> list;

            public GUIWrapper(object localAssets, Dictionary<string, int> idMap, List<(string, Rect)> list)
            {
                this.localAssets = localAssets;
                this.idMap = idMap;
                this.list = list;
            }

            public void OnProjectGUI(string guid, Rect badRect)
            {
                if (IsInRightColumn(guid,idMap))
                {
                    list.Add((guid, ConvertItemRect(badRect)));
                }
            }
        }
        
        #if UNITY_2022_1_OR_NEWER
        
        public static Dictionary<string,int> GetCurrentIds(object localAssets)
        {
            var ids = localAssets.InvokeMethod<List<KeyValuePair<string, int>>>("GetVisibleNameAndInstanceIDs");

            var thing = new NativeArray<int>(ids.Select(kv => kv.Value).ToArray(), Allocator.Temp);
            var output = new NativeArray<GUID>(thing.Length, Allocator.Temp);
            
            #pragma warning disable CS0618 // Type or member is obsolete
            AssetDatabase.InstanceIDsToGUIDs(thing,output);
            #pragma warning restore CS0618 // Type or member is obsolete
            var normalThing = thing.ToArray();
            var normalOutput = output.ToArray();

            var result = new Dictionary<string, int>(normalOutput.Distinct().Select((id, i) => new KeyValuePair<string, int>(id.ToString(), normalThing[i])));

            return result;
        }
        
        public static IEnumerator GetDisplayedRects(List<(string, Rect)> list)
        {
            var localAssets = LoadLocalAssets();
            var idMap = GetCurrentIds(localAssets);

            var wrapper = new GUIWrapper(localAssets, idMap, list);
            
            EditorApplication.projectWindowItemOnGUI += wrapper.OnProjectGUI;
            UnityWindows.GetWindows(Window.ProjectBrowser).ForEach(b => b.Repaint());
            
            for (int i = 0; i < 1000; i++)
            {
                if (list.IsEmpty())
                {
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            EditorApplication.projectWindowItemOnGUI -= wrapper.OnProjectGUI;
            Assert.IsNotEmpty(list);
        }
        
        public static void NoItemPosition(List<Vector2> list, List<Rect> rects) //this is wrong when the left column has scroll offset
        {
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var rect in rects.Shuffled())
            {
                maxX = Mathf.Max(maxX, rect.xMax);
                maxY = Mathf.Max(maxY, rect.yMax);
            }
            
            //adding 20 to avoid clicking the little arrow that opens some parts of the asset
            list.Add(new Vector2(maxX + 1, maxY + 20));
        }
        
        // public static IEnumerator ArbitraryItemPosition(List<(string, Vector2)> list)
        // {
        //     var rects = new List<(string, Rect)>();
        //     var enumerator = GetDisplayedRects(rects);
        //
        //     while (rects.IsEmpty())
        //     {
        //         enumerator.MoveNext();
        //         yield return null;
        //     }
        //     
        //     var rect = rects.Random();
        //     var vec = new Vector2(rect.Item2.xMax - 20, rect.Item2.yMax + 20)
        //     list.Add((rect.Item1,vec));
        //     
        // }
        
        #endif
        
    }
}