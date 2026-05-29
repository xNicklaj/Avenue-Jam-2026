using System;
using UnityEngine;

namespace TabbyStudios
{
    public class TabbyWindow<T> : TabbyWindow where T : TabbyWindow<T> 
    {
        protected static T CreateInstance()
        {
            T window = ScriptableObject.CreateInstance<T>();
            window.positioner = new WindowPositioner(window);
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
            window.PrivateOnCreate(pos);
            return window;
        }
        
        public static T Create(Rect rect)
        {
            var window = CreateInstance();
            window.OnCreate(rect);
            return window;
        }
     
        public static T Launch()
        {
            var window = Create();
            window.Display();
            return window;
        }
        
        private void PrivateOnCreate(Vector2 pos)
        {
            try
            {
                OnCreate(pos);
            }
            catch (Exception e)
            {
                DestroyImmediate(this);
                throw;
            }
        }
        
        private void PrivateOnCreate()
        {
            try
            {
                OnCreate();
            }
            catch (Exception e)
            {
                DestroyImmediate(this);
                throw;
            }
        }
        
        public virtual void OnCreate()
        {
            
        }
        
        public virtual void OnCreate(Vector2 pos)
        {
            
        }
        
        public virtual void OnCreate(Rect pos)
        {
           //???? 
        }
    }
}