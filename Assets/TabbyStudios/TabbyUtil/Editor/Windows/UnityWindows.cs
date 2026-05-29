#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public enum Window
    {
        ProjectBrowser, Console, SceneView, Inspector, TestRunner, UIBuilder, Hierarchy, ObjectSelector, ColorPicker
    }
    
    public static class UnityWindows
    {
        private static Dictionary<string,string> fixedNames = new()
        {
            {"ProjectBrowser", "ProjectBrowser"},
            {"Console", "ConsoleWindow"},
            {"SceneView", "SceneView"},
            {"Inspector", "InspectorWindow"},
            {"TestRunner", "TestRunnerWindow"},
            {"ObjectSelector", "ObjectSelector"},
            {"UIBuilder", "Builder"},
            {"Hierarchy", "SceneHierarchyWindow"},
            {"ColorPicker", "ColorPicker"},
        };

        public static EditorWindow projectBrowser => LastFocused(Window.ProjectBrowser);
        public static EditorWindow hierarchy => LastFocused(Window.Hierarchy);
        public static EditorWindow console => LastFocused(Window.Console);
        public static EditorWindow uiBuilder => LastFocused(Window.UIBuilder);
        public static EditorWindow sceneView => LastFocused(Window.SceneView);
        public static EditorWindow inspector => LastFocused(Window.Inspector);
        public static EditorWindow testRunner => LastFocused(Window.TestRunner);
        
        public static EditorWindow objectSelector => GetWindow(Window.ObjectSelector);
        public static EditorWindow colorPicker => GetWindow(Window.ColorPicker);
        
        public static Action onFocusChanged { get; set; } = () => { };
        
        private static EditorWindow lastFocusedWindow;
        private static Dictionary<string, WeakReference<EditorWindow>> focusMap = new();
        
        static UnityWindows()
        {
            EditorUtil.CallEveryXUpdates(10, TrackFocus);
        }
        
        public static EditorWindow GetWindow(Window window)
        {
            return GetWindows(window).FirstOrDefault();
        }
        
        public static T GetWindow<T>() where T : EditorWindow
        {
            return GetWindows().OfType<T>().FirstOrDefault();
        }
        
        public static List<EditorWindow> GetWindows(Window window)
        {
            var windows = GetWindows();
            var found = windows.Where(w => w.GetType().Name == fixedNames[window.ToString()]).ToList();
            EnsureNameNotAmbiguous(found);
            return found;
        }
        
        public static List<T> GetWindows<T>() where T : EditorWindow
        {
            return GetWindows().OfType<T>().ToList();
        }
        
        public static List<EditorWindow> GetWindows()
        {
            return Resources.FindObjectsOfTypeAll<EditorWindow>().ToList();
        }

        //test this
        // public static EditorWindow LastFocused<T>() where T : EditorWindow
        // {
        //     return focusMap.GetValueOrDefault(typeof(T).Name, null)?.Target() ?? GetWindow<T>();
        // }
        
        public static EditorWindow LastFocused(Window window)
        {
            return focusMap.GetValueOrDefault(window.ToString(), null)?.Target() ?? GetWindow(window);
        }
        
        public static bool IsOpen(this EditorWindow window)
        {
            return (bool)typeof(EditorWindow).InvokeStaticGenericMethod("HasOpenInstances", window.GetType(), null);
        }
        
        public static bool IsClosed(this EditorWindow window)
        {
            return !window.IsOpen();
        }
        
        
        private static void EnsureNameNotAmbiguous(List<EditorWindow> found)
        {
            Assert.IsTrue(found.All(w => w.GetType() == found.First().GetType()));
        }
        
        public static void TrackFocus()
        {
            var focusChanged = lastFocusedWindow is not null && !HasFocus(lastFocusedWindow);
            lastFocusedWindow = EditorWindow.focusedWindow;
            if (lastFocusedWindow is null)
                return;

            var w = focusMap.GetValueOrDefault(lastFocusedWindow.GetType().Name);
            
            if (w is null || w.Target() != lastFocusedWindow)
            {
                focusMap[lastFocusedWindow.GetType().Name] = new WeakReference<EditorWindow>(lastFocusedWindow);
            }
            
            if (focusChanged)
            {
                //should we also trigger when focusing for the first time?
                onFocusChanged();
            }
        }

        public static bool HasFocus(this EditorWindow window)
        {
            return EditorWindow.focusedWindow == window;
        }
        
        public static bool EditorMinimized()
        {
            return !InternalEditorUtility.isApplicationActive;
        }
        
    }
}

#endif