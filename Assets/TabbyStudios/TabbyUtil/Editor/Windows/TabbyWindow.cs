using UnityEngine;

namespace TabbyStudios
{
    public class TabbyWindow<T> : TabbyWindow where T : TabbyWindow<T> 
    {
        protected static T CreateInstance()
        {
            var before = mouseOverWindow;
            T window = ScriptableObject.CreateInstance<T>();
            window.mouseOverWindowBeforeOpen = before;
            windows.Add(window);
            return window;
        }
    
        public static T Create()
        {
            var window = CreateInstance();
            window.PrivateOnCreate();
            return window;
        }
    
        public static T Create(Vector2 pos)
        {
            var window = CreateInstance();
            window.pos = pos;
            window.PrivateOnCreate();
            return window;
        }
        
        public static T Create(Rect rect)
        {
            var window = CreateInstance();
            window.pos = rect.position;
            window.size = rect.size;
            window.OnCreate();
            return window;
        }
     
        public static T Launch()
        {
            var window = Create();
            window.Display();
            return window;
        }
        
        private void PrivateOnCreate()
        {
            try
            {
                OnCreate();
            }
            catch
            {
                DestroyImmediate(this);
                throw;
            }
        }
        
        public virtual void OnCreate()
        {
            
        }
    }
}