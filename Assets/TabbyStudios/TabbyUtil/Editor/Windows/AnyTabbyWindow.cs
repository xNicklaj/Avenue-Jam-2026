using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public class TabbyWindow : EditorWindow, AbstractScreenSpace
    {
        private Vector2 lastSize;
        protected static List<TabbyWindow> windows;
        protected WindowPositioner positioner;

        public Mode defaultMode = Mode.Default;
        public bool notDisposed;

        public bool isActuallyDisplaying => !float.IsNaN(rootVisualElement.resolvedStyle.width); 
        
        static TabbyWindow() 
        {
            windows = new();
            EditorApplication.wantsToQuit += OnQuit;
            AssemblyReloadEvents.beforeAssemblyReload += ClearWindows;
            EditorUtil.CallEveryXUpdates(20, TryCallOnResizeOnWindows); //todo add proper framerate;
            //EditorUtil.CallEveryXUpdates(5000, ClearNullWindows);
        }
        
        public void Display(Mode mode = Mode.Default)
        {
            ShowWithMode(mode == Mode.Default ? defaultMode : mode);
            positioner.OnEnable();
            rootVisualElement.Embed(this);
        }
        
        public IEnumerator DisplayAndWait(Mode mode = Mode.Default)
        {
            Display(mode);
            yield return RunAsync.WaitUntil(() => isActuallyDisplaying);
            yield return null;
        }
        
        public void Dispose()
        {
            if (notDisposed)
            {
                OnDisable();
                Close();
            }
        }
        
        public static void ClearWindows()
        {
            windows?.ForEach(w => w?.Dispose());
        }
        
        public static void ClearNullWindows()
        {
            var toRemove = windows.Select((w,i) => w == null ? i : -1).Where(i => i != -1).ToList();
            //Debug.Log(toRemove.Count);
            foreach (var i in toRemove)
            {
                windows.RemoveAt(i);
            }
        }
        
        private void ShowWithMode(Mode mode)
        {
            if (mode == Mode.Default)
                Show();
            else if(mode == Mode.Popup)
                ShowPopup();

            notDisposed = true; //Unity will not let you close a window that wasn't shown
        }
        
        public Vector2 ScreenPosition(VisualElement e)
        {
            var window = e.ContainingWindow();
            if (window is null)
                return Vector2.zero;
            
            // if(window is TabbyWindow w)
            //     return (Vector2)e.worldTransform.GetPosition() + w.screenPosition;
            
            return (Vector2)e.worldTransform.GetPosition() + window.position.position;

        }

        private static void TryCallOnResizeOnWindows()
        {
            foreach (var w in UnityWindows.GetWindows<TabbyWindow>())
            {
                if (w.lastSize != w.position.size)
                {
                    w.OnResize();
                    w.lastSize = w.position.size;
                }
            }
        }

        public virtual void OnResize()
        {
            
        }
        
        public static bool OnQuit()
        {
            ClearWindows();
            return true;
        }

        public void OnDestroy()
        {
            //windows.Remove(this);
            OnOnDestroy();
        }

        public virtual void OnOnDestroy()
        {
            
        }
        

        public virtual void OnDisable()
        {
            //this needs to be called instead of OnDispose so unity can call it in cases where we don't control the closing of the window
            //e.g. clicking the x on the top right
            if (notDisposed)
            {
                notDisposed = false;
                rootVisualElement.Disable();
            } 
        }
        
        public virtual void OnLostFocus()
        {
        
        }
        
        public enum Mode
        {
            Default, Popup,
        }
        
        public Vector2 screenPosition
        {
            get => positioner.screenPosition;
            set => positioner.screenPosition = value;
        }
        
        public Vector2 size
        {
            get => positioner.size;
            set => positioner.size = value;
        }

        public Rect rect
        {
            get => positioner.rect;
            set
            {
                positioner.screenPosition = value.position;
                positioner.size = value.size;
            }
        }
        
    }
    
}