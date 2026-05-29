using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class CustomMenu : VisualComponent, RadioManager<CustomMenuEntry>
    {
        public string path;
        public MenuManager manager;
        public ItemProvider provider;
        public List<CustomMenuEntry> items => scroller.items.Select(i => i.SelectFirstOrDefaultComponent<CustomMenuEntry>()).Where(i => i is not null).ToList();
        private AbstractScroller scroller;

        private Texture2D roundedCornerBackground;
        
        public CustomMenu(MenuManager manager, string path, ItemProvider provider)
        {
            this.manager = manager;
            this.path = path;
            this.provider = provider;
        }
    
        //must move to Start Eventually otherwise we must always add provider BEFORE  custom menu
        public override void Awake()
        {
            RegisterGeometryChanged();
            TrySetRoundedCorners();
            
            scroller = target.SelectFirstComponent<AbstractScroller>();
            
            target.style.backgroundColor = UnityColors.defaultColor;
            provider.CreateMenuBehaviour();
            var createdItems = provider.GetItems(path);
            var max = Config.instance.GetInt(nameof(TabbyConfig.maxItemsPerMenu));

            int i = 0;
            foreach (var item in createdItems)
            {
                item.parentMenu = this;
                var elem = item.CreateParent();
                elem.AddComponent(item);

                provider.AddOtherComponents(item);
                scroller.AddItem(item.target);

                i++;
                if (max > 0 && i >= max) break;
            }
        }

        public bool TryHideBigWindowWhenUnityMinSizeIsTooBig(Vector2 windowSize) // this is called from fix position, when size not resolved
        {
            if (target.ContainingWindow() as TabbyWindow is { } w)
            {
                if (windowSize.x < ScreenPixels.minWindowSize.x || windowSize.y < ScreenPixels.minWindowSize.y)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetBackgroundPixels(Rect screenLocation)
        {
            roundedCornerBackground = ScreenPixels.ReadPixels(screenLocation);
            target.ContainingWindow().rootVisualElement.style.backgroundImage = roundedCornerBackground;
        }

        public bool TryStartOpacityTransition()
        {
            var round = Config.instance.GetBool(nameof(TabbyConfig.roundedMenuCorners));
            var fade = Config.instance.GetBool(nameof(TabbyConfig.menuFadein));
            
            if (!fade) return false;
            
            var duration = 0.15f;
            target.style.transitionDuration = new List<TimeValue> { duration };
            target.style.transitionProperty = new List<StylePropertyName> { "opacity" };
            target.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.Linear };
            
            target.style.opacity = 0;
            foreach (var t in target.SelectComponent<TextComponent>())
            {
                t.opacity = 0;
            }

            FadeIn(target, duration);
            return true;
        }
        
        public bool TrySetRoundedCorners()
        {
            if (!Config.instance.GetBool(nameof(TabbyConfig.roundedMenuCorners))) return false;
            target.style.borderBottomLeftRadius = 6;
            target.style.borderBottomRightRadius = 6;
            target.style.borderTopLeftRadius = 6;
            target.style.borderTopRightRadius = 6;
            return true;
        }

        private void FadeIn(VisualElement element, float duration)
        {
            double elapsed = 0;
            double last = EditorApplication.timeSinceStartup;
            element.schedule.Execute(() =>
            {
                elapsed += EditorApplication.timeSinceStartup - last;
                last = EditorApplication.timeSinceStartup;

                foreach (var t in target.SelectComponent<TextComponent>())
                {
                    t.opacity = element.resolvedStyle.opacity;
                }

            }).Every(0).Until(() => elapsed >= duration);
        }
        
        public CustomMenuEntry Next(CustomMenuEntry item)
        {
            var index = items.IndexOf(item) + 1;
            return index < items.Count ? items[index] : null;
        }

        public void MoveSelection(int d)
        {
            var actualItems = items.Where(i => i is Item || i is SearchItem).ToList();
            if (actualItems.IsNullOrEmpty())
                return;


            var index = 0;
            var selected = actualItems.FirstOrDefault(item => item.selected);
            if (selected is not null)
                index = actualItems.IndexOf(selected) + d;
            
            if (index >= actualItems.Count || index < 0)
                return;
            
            actualItems[index].InvokeMethod("Select");
        }

        public void SelectCurrent()
        {
            var actualItems = items.Where(i => i is Item or SearchItem && i.GetFieldValue<bool>("selected")).ToList();
            if (actualItems.IsNullOrEmpty())
                return;

            actualItems.First().GetComponent<ItemComponent>().OnMouseEnter(null);
            actualItems.First().GetComponent<ItemComponent>().DoItemAction();
        }
    
        public CustomMenuEntry NextVisible(CustomMenuEntry item)
        {
            return items.FirstOrDefault(i => i.data.priority > item.data.priority && i.target.visible && i != item);
        }

        public void UncheckOthers(CustomMenuEntry t)
        {
            items.Where(i => i is Item || i is SearchItem).Where(item => item != t).ForEach(item => item.InvokeMethod("Deselect"));
        }

        public void ClearElements()
        {
            target.Pluck();
            target.Disable();
        }

        public void Refresh()
        {
            scroller.Clear();
            Awake();
        }

        public override void OnGeometryChanged(GeometryChangedEvent e)
        {
            if (Config.instance.GetBool(nameof(TabbyConfig.menuFadein))) target.style.opacity = 1;
        }

        public override void OnDisable()
        {
            if (roundedCornerBackground != null)
            {
                Object.DestroyImmediate(roundedCornerBackground);
            }
        }
    }
}