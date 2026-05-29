using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public class TabbyWindow : EditorWindow, AbstractScreenSpace
    {
        protected static List<TabbyWindow> windows;

        public Mode defaultMode = Mode.Default;
        public bool notDisposed;
        public bool didStyleResolve => !float.IsNaN(rootVisualElement.resolvedStyle.width);
        protected EditorWindow mouseOverWindowBeforeOpen;
        
        static TabbyWindow() 
        {
            windows = new();
            EditorApplication.wantsToQuit += OnQuit;
            AssemblyReloadEvents.beforeAssemblyReload += ClearWindows;
        }

        protected virtual void BeforeDisplay()
        {
            
        }
        
        public void Display(Mode mode = Mode.Default)
        {
            BeforeDisplay();
            ShowWithMode(mode == Mode.Default ? defaultMode : mode);
            rootVisualElement.Embed(this);
            position = new Rect(pos, size);
        }
        
        public IEnumerator DisplayAndWait(Mode mode = Mode.Default)
        {
            Display(mode);
            yield return RunAsync.WaitUntil(() => didStyleResolve);
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
        
        public static bool OnQuit()
        {
            ClearWindows();
            return true;
        }

        public virtual void OnDestroy()
        {
            //windows.Remove(this);
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
        
        private Vector2 _pos;
        public Vector2 pos
        {
            protected get => _pos;
            set
            {
                if (!didStyleResolve) _pos = value;
                else position = new Rect(value, resolvedSize);
            }
        }

        private Vector2 _size;
        public Vector2 size
        {
            protected get => _size;
            set
            {
                if (!didStyleResolve) _size = value;
                else position = new Rect(resolvedPos, value);
            }
        }
        
        public Vector2 resolvedPos => didStyleResolve ? position.position : new Vector2(float.NaN, float.NaN);
        public Vector2 resolvedSize => didStyleResolve ? position.size : new Vector2(float.NaN, float.NaN);
    }
    
}