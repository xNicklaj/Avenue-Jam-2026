using System;
using UnityEngine;

namespace TabbyStudios
{
    public class WindowPositioner
    {
        private TabbyWindow window;
        public Action fitToScreen;
    
        public WindowPositioner(TabbyWindow window)
        {
            this.window = window;
        }

        private Rect _rect;
        public Rect rect
        {
            get
            {
                return _rect;
            }
            set
            {
                _rect = value;
                UpdateUnityRect();
            
            }
        }
        
        protected bool screenPositionSet;
        public Vector2 screenPosition
        {
            get => rect.position;
            set
            {
                rect = new Rect(value, rect.size);
                screenPositionSet = true;
                if(sizeSet && !Config.GetSetting<bool>("multiMonitorFix"))
                    FitToScreen();
            }
        }
        
        protected bool sizeSet;
        public Vector2 size
        {
            get => rect.size;
            set
            {
                rect = new Rect(rect.position, value);
                sizeSet = true;
                if(screenPositionSet)
                    FitToScreen();
            }
        }

        public void OnEnable()
        {
            UpdateUnityRect();
        }

        private void UpdateUnityRect()
        {
            window.position = new Rect(screenPositionSet ? rect.position : window.position.position, sizeSet ? rect.size : window.position.size);
        }
    
        public Vector2 Offset(Vector2 attemptedPosition)
        {
            
            var offset = new Vector2(attemptedPosition.x + size.x - (EditorUtil.screenSize.x - EditorUtil.windowMinDistanceToScreenEdge), 0);
            offset = new Vector2(Mathf.Max(offset.x, 0), 0);
            return offset;
        }

        public Rect DefaultFitRect()
        {
            return new Rect(screenPosition - Offset(screenPosition), size);
        }
    
        private void FitToScreen()
        {
            rect = DefaultFitRect();
            if (fitToScreen is not null)
                fitToScreen();
        }
    
    }
}