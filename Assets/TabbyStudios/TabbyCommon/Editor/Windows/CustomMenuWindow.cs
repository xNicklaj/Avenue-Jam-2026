using System;
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
        private static float scale => Config.instance.GetInt("menuScale") / 100f;
        
        private bool isBaseWindow;
        private Rect parentWindowRect;
        
        public override void OnCreate()
        {
            defaultMode = Mode.Popup;
            EditorApplication.delayCall += Focus;
        }

        protected override void BeforeDisplay()
        {
            FixPosition();
        }
        
        private void FixPosition()
        {
            var windows = UnityWindows.GetWindows<CustomMenuWindow>().Except(this).ToList();

            if (!menu.path.Contains("/")) 
            {
                isBaseWindow = true;
                if (TryGetParentWindowRect(out var rect))
                {
                    parentWindowRect = rect;
                }
            }
            else
            {
                parentWindowRect = windows.FirstOrDefault(w => w.isBaseWindow)?.parentWindowRect ?? default;
                if (parentWindowRect.width == 0 && parentWindowRect.height == 0) return;
            }
            
            if (isBaseWindow)
            {
                var x = Math.Clamp(pos.x, float.NegativeInfinity, parentWindowRect.xMax - size.x);
                var y = Math.Clamp(pos.y, float.NegativeInfinity, parentWindowRect.yMax - size.y);
                pos = new Vector2(x,y);
            }
            else 
            {
                var w = windows.WithMax(w => w.menu?.path.Count("/") ?? float.NegativeInfinity);
            
                if (pos.x + size.x > parentWindowRect.xMax && !float.IsNaN(w.resolvedPos.x))
                {
                    pos = new Vector2(w.resolvedPos.x - size.x, pos.y);
                }
                if (pos.y + size.y > parentWindowRect.yMax)
                {
                    pos = new Vector2(pos.x, Math.Clamp(pos.y, float.NegativeInfinity, parentWindowRect.yMax - size.y));
                }
            }

            if (menu.TryStartOpacityTransition() || menu.TrySetRoundedCorners() || menu.TryHideBigWindowWhenUnityMinSizeIsTooBig(size))
            {
                menu.SetBackgroundPixels(new Rect(pos, size));
            }
        }

        private bool TryGetParentWindowRect(out Rect parentRect)
        {
            Rect largestX = new Rect();
            foreach (var w in UnityWindows.GetWindows())
            {
                if (w == this) continue;
                var rect = w.GetFieldValue("m_Parent")?.GetPropertyValue("window")?.GetPropertyValue<Rect>("position") ?? default;
                if (rect.xMin <= pos.x && rect.xMax > largestX.xMax)
                {
                    largestX = rect;
                }
            }

            if (mouseOverWindowBeforeOpen == null || mouseOverWindowBeforeOpen is null)
            {
                parentRect = default;
                return false;
            }
            
            var p = mouseOverWindowBeforeOpen.GetFieldValue("m_Parent").GetPropertyValue("window").GetPropertyValue<Rect>("position");
            parentRect = new Rect(largestX.x, p.y, largestX.width, p.height);
            return true;
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
            var windows = UnityWindows.GetWindows<CustomMenuWindow>();
            if(mouseOverWindow == this || windows.Count(w => w.rootVisualElement.SelectFirstComponent<SearchBar>().IsEnabled()) == 1
                                       ||(windows.None(w => mouseOverWindow == w) && this.HasFocus()))
            {
                searchBar.CharacterTypedWithoutFocus(key);
            }
        }

        public static Vector2 CreatePos(CustomMenuEntry caller, int dir)
        {
            //must use scaled value and unify left and right creation
            var screenPos = caller.target.ScreenPosition();
            var space = caller.target.EmptySpaceTo(caller.target.GetComponentUpwards<CustomMenu>().target);
            return new Vector2(screenPos.x + dir*(caller.target.Width() + menuOverlap*scale), screenPos.y - space);
        }
    }
}

