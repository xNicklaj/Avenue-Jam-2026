using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public class CustomMenuWindow : TabbyWindow<CustomMenuWindow>
    {
        public CustomMenu menu;
        
        private static TabbyInput input = TabbyInput.Input(1);

        public static float menuOverlap = 3;
        private static float scale => Config.GetSetting<int>("menuScale") / 100f;

        private static int maxHeight => Config.GetSetting<int>("maxMenuHeight");
        private static int maxWidth => Config.GetSetting<int>("maxMenuWidth");
        private bool allowOverlap;
        
        public override void OnCreate(Vector2 pos)
        {
            defaultMode = Mode.Popup;

            if(!Config.GetSetting<bool>("multiMonitorFix")) 
                positioner.fitToScreen = FixPosition;

            TryFixBlinking();
            screenPosition = pos;
            
            EditorApplication.delayCall += Focus;
            
        }
        
        private Tuple<float,float> CalculateInterval(CustomMenuWindow exclude)
        {
            return CalculateInterval(new List<CustomMenuWindow>{exclude});
        }
        
        private Tuple<float,float> CalculateInterval(IEnumerable<CustomMenuWindow> exclude)
        {
            var m = UnityWindows.GetWindows<CustomMenuWindow>().Except(exclude).ToList();
            if (m.IsNullOrEmpty())
                return new(0, 0);
            var max = m.Max(menu => menu.position.x + menu.menu.target.Width());
            var min = m.Min(menu => menu.position.x);
            return new(min, max);
        }

        private bool IsInside(Tuple<float,float> interval)
        {
            var pos = positioner.DefaultFitRect();
            return pos.x >= interval.Item1 && pos.x <= interval.Item2 - 8*scale;
        }
        
        private void FixPosition()
        {
            //we cant use screenposition because we are in the middle of fixing screenposition
            var interval = CalculateInterval(this);
            if (IsInside(interval))
            {
                positioner.rect = new Rect(new Vector2(interval.Item1 - position.size.x*scale + 8*scale, screenPosition.y), position.size);
            }
        }

        private void OnGUI()
        {
            if (input.key == KeyCode.Escape)
            {
                EditorInputHandler.manager.ClearMenus();
            }
            else if (KeyUtil.IsTypingKey(input.key))
            {
                UnityWindows.GetWindows<CustomMenuWindow>().ForEach(w => w.KeyTyped(input.key));
                input.current.Use();
            }
            else
            {
                TryHandleArrows(input.key);
            }
        }

        public void TryHandleArrows(KeyCode key)
        {
            if(key == KeyCode.UpArrow)
            {
                menu.MoveSelection(-1);
            }
            else if(key == KeyCode.DownArrow)
            {
                menu.MoveSelection(1);
            }
            else if(key is KeyCode.Return or KeyCode.KeypadEnter)
            {
                menu.SelectCurrent();
            }
        }

        public void KeyTyped(KeyCode key)
        {
            var searchBar = rootVisualElement.SelectFirstComponent<SearchBar>();
            if(this.HasFocus() || UnityWindows.GetWindows<CustomMenuWindow>().Count(w => w.rootVisualElement.SelectFirstComponent<SearchBar>().IsEnabled()) == 1)
            {
                searchBar.CharacterTypedWithoutFocus(key);
            }
        }

        public static Vector2 CreatePos(CustomMenuEntry caller, int dir)
        {
            //todo must use scaled value and unify left and right creation
            var screenPos = caller.target.ScreenPosition();
            var space = caller.target.EmptySpaceTo(caller.target.GetComponentUpwards<CustomMenu>().target);
            return new Vector2(screenPos.x + dir*(caller.target.Width() + menuOverlap*scale), screenPos.y - space);
        }
        
        private void TryFixBlinking()
        {
            //here we set the size just to attempt to predict the size of the window to make it stop blinking, later we try to figure out how to precompute it
            //the menus work the same way if we don't set the size here
            size = new Vector2(maxWidth, maxHeight);
        }
    }
}

