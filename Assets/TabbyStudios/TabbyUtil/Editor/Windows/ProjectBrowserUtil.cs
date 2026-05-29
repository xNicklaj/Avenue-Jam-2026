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
            if (focusedWindow?.TypeName() == "ProjectBrowser")
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
            var offset = new Vector2(LeftColumnWidth(), 25);
            return new Rect(r.position + offset, r.size);
        }

        public static void SetOpenFolder(string path)
        {
            int id = AssetDatabase.LoadAssetAtPath<Object>(path).GetInstanceID();
            UnityWindows.projectBrowser?.InvokeMethod("ShowFolderContents", id, true); 
        }
        
        
        //be careful these are extremely slow they're mostly for testing
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        private class GUIWrapper
        {
            private object localAssets;
            private Dictionary<string, int> idMap;
            private List<Rect> list;

            public GUIWrapper(object localAssets, Dictionary<string, int> idMap, List<Rect> list)
            {
                this.localAssets = localAssets;
                this.idMap = idMap;
                this.list = list;
            }

            public void OnProjectGUI(string guid, Rect badRect)
            {
                if (IsInRightColumn(guid,idMap))
                {
                    list.Add(ConvertItemRect(badRect));
                }
            }
        }
        
        #if UNITY_2022_1_OR_NEWER
        
        public static Dictionary<string,int> GetCurrentIds(object localAssets)
        {
            var ids = localAssets.InvokeMethod<List<KeyValuePair<string, int>>>("GetVisibleNameAndInstanceIDs");

            var thing = new NativeArray<int>(ids.Select(kv => kv.Value).ToArray(), Allocator.Temp);
            var output = new NativeArray<GUID>(thing.Length, Allocator.Temp);
            AssetDatabase.InstanceIDsToGUIDs(thing,output);
            var normalThing = thing.ToArray();
            var normalOutput = output.ToArray();

            var result = new Dictionary<string, int>(normalOutput.Distinct().Select((id, i) => new KeyValuePair<string, int>(id.ToString(), normalThing[i])));

            return result;
        }
        
        public static IEnumerator GetDisplayedRects(List<Rect> list)
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
        
        public static IEnumerator NoItemPosition(List<Vector2> list) //this is wrong when the left column has scroll offset
        {
            var rects = new List<Rect>();
            var enumerator = GetDisplayedRects(rects);

            while (rects.IsEmpty())
            {
                enumerator.MoveNext();
                yield return null;
            }
            
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
        
        public static IEnumerator ArbitraryItemPosition(List<Vector2> list)
        {
            var rects = new List<Rect>();

            var enumerator = GetDisplayedRects(rects);

            while (rects.IsEmpty())
            {
                enumerator.MoveNext();
                yield return null;
            }
            
            var rect = rects.Random();
            var vec = new Vector2(rect.xMax - 20, rect.yMax + 20);
            list.Add(vec);
            
        }
        
        #endif
        
    }
}