#define ASDF

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public class TabbyInput
    {
        
        public Event current => Event.current ?? last;
        public Event last;

        public bool isClick => current.type is EventType.MouseDown or EventType.MouseUp; 
        public int button => current.clickCount > 0  ? current.button : -1;
        public bool isPossibleClick => current.clickCount > 0;

        public KeyCode key => current.type == EventType.KeyDown ? current.keyCode : KeyCode.None;
        
        private Dictionary<int, ButtonData> buttonMap = new();

        private bool clickCheckedThisFrame, possibleClickCheckedThisPeriod, properEventSeen;
        private bool dragCheckedThisFrame;
        private int period, storedButton;

        private Vector2 lastPos;
        
        public Action onLeftMouseDown = () => {};
        public Action onLeftMouseUp = () => {};
        public Action onRightMouseDown = () => {};
        public Action onRightMouseUp = () => {};
        public Action onUnityContextClick = () => { };
        public Action onUnityUnwantedContextClick = () => { };
        public Action onMouseDrag = () => { };

        public static TabbyInput Input(int period, params EditorWindow[] windows)
        {
            var input = new TabbyInput();
            input.period = period;
            AssignCallbacks(input,windows);
            return input;
        }
        
        public static TabbyInput Input(params EditorWindow[] windows)
        {
            return Input(1, windows);
        }

        private static void AssignCallbacks(TabbyInput input, EditorWindow[] windows)
        {
            if (windows.IsEmpty())
            {
                EditorApplication.projectWindowItemOnGUI += (s,r) => input.Update();
                EditorApplication.hierarchyWindowItemOnGUI += (s,r) => input.Update();
            }
            else if(windows.Any(w => w?.GetType().Name == "ProjectBrowser"))
                EditorApplication.projectWindowItemOnGUI += (s,r) => input.Update();
            else if(windows.Any(w => w?.GetType().Name == "SceneHierarchyWindow"))
                EditorApplication.hierarchyWindowItemOnGUI += (s,r) => input.Update();
        }

        private TabbyInput()
        {
            
        }
        
        public void Use()
        {
            current.Use();
        }
        
        public void Update()
        {
            if (!possibleClickCheckedThisPeriod && isPossibleClick)
            {
                last = current;
                storedButton = button;
                FlipMousePossiblyDown(button);
                WaitForProperClick();
            }
            
            //no else is correct
            if (!clickCheckedThisFrame && isClick)
            {
                HandleRealClick(); 
                DontClickAgainThisFrame();
            }
            
            else if (IsMouseDown(0) && !dragCheckedThisFrame && current.mousePosition != lastPos)
            {
                onMouseDrag();
                DontDragAgainThisFrame();
            }

            lastPos = current.mousePosition;
        }
        
        private bool IsMouseUpEvent()
        {
            return current.type == EventType.MouseUp;
        }
        
        private bool IsMouseDownEvent()
        {
            return current.type == EventType.MouseDown;
        }
        
        private void HandleRealClick()
        {
            properEventSeen = true;
            if (IsMouseDownEvent())
                OnMouseDown(button);
            else
                OnMouseUp(button);

        }
        
        private void HandleInferredClick()
        {
            if (!IsMouseDown(storedButton))
                OnMouseDown(storedButton);
            else
                OnMouseUp(storedButton);
        }

        private void OnMouseDown(int button)
        {
            SetMouseDown(button,true);
            if (button == 0)
                onLeftMouseDown();
            else if (button == 1)
                onRightMouseDown();
        }
        
        private void OnMouseUp(int button)
        {
            SetMouseDown(button, false);
            if (button == 0)
                onLeftMouseUp();
            else if (button == 1)
                onRightMouseUp();
        }
        
        public bool IsMouseDown(int button)
        {
            return buttonMap.SelectValueOrDefault(button, b => b.isDown, false);
        }
        
        public void SetMousePossiblyDown(int button,bool down)
        {
            buttonMap.SafeGet(button).isPossiblyDown = down;
        }
        
        public void SetMouseDown(int button, bool down)
        {
            buttonMap.SafeGet(button).isDown = down;
            SetMousePossiblyDown(button,down);
        }
        
        public void FlipMouseDown(int button)
        {
            SetMouseDown(button, !IsMouseDown(button));               
        }
        
        public void FlipMousePossiblyDown(int button)
        {
            buttonMap.SafeGet(button).isPossiblyDown = !buttonMap.SafeGet(button).isPossiblyDown;
        }
        

        public bool IsUnityContextClick()
        {
            return current.type == EventType.ContextClick || (current.type == EventType.MouseUp && button == 1);
        }
        
        private void WaitForProperClick()
        {
            possibleClickCheckedThisPeriod = true;
            EditorUtil.UpdateDelayCall(period+1, () =>
            {
                possibleClickCheckedThisPeriod = false;
                if (!properEventSeen)
                {
                    HandleInferredClick();
                }
                properEventSeen = false;
                
            });
        }
        
        private void DontClickAgainThisFrame()
        {
            clickCheckedThisFrame = true;
            EditorUtil.UpdateDelayCall(period, () => clickCheckedThisFrame = false);
        }
        
        private void DontDragAgainThisFrame()
        {
            dragCheckedThisFrame = true;
            EditorUtil.UpdateDelayCall(period, () => dragCheckedThisFrame = false);
        }
        
        
        private class ButtonData
        {
            public int button;
            public bool isDown, isPossiblyDown;
            
            public ButtonData(int button)
            {
                this.button = button;
            }
            
            public ButtonData()
            {
                
            }
        }
    }
    
}