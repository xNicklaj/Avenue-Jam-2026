using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class SettingsMenuManager : MenuManager
    {
        private static string xmlName = "CustomMenu";
        
        public List<CustomMenu> menus { get; set; } = new();

        private VisualElement menuContainer;
        public List<string> menuPaths = new();
        public bool fixLastItem = false;

        private bool showAsPreview;
    
        public SettingsMenuManager(VisualElement root)
        {
            menuContainer = root.Q("SettingsMenuContainer");
            Config.Subscribe<bool>("previewButton", Refresh2);
            Config.Subscribe<float>("settingsMenuZoom", SetScale, callImmediately:true);
            Config.Subscribe<bool>("onMenusModified", Refresh2);
            Config.Subscribe<bool>("expandTopLevelMenus", Refresh3);
        }
        
        public CustomMenu CreateMenu(string path, Vector2 relativePos)
        {
            var xml = AssetCache.LoadXml(xmlName);
            var menuElement = xml.First("CustomMenu");
            var provider = menuElement.AddComponent<SettingsItemProvider>();
            var menu = menuElement.AddComponent<CustomMenu>(this, path ,provider);
            xml.SetSize(Config.GetSetting<int>("maxMenuWidth"), Config.GetSetting<int>("maxMenuHeight"));
            menuContainer.AddElement(xml);
            
            if (ExtraData.nonCustomizablePaths.Contains(path))
                BlockCustomization(xml, path);
            
            var area = menu.target;
            // area.parent.style.overflow = Overflow.Hidden;
            // area.parent.style.width = new Length(100, LengthUnit.Percent);
            area.style.position = Position.Absolute;
            area.SetAbsolutePositionRelative(relativePos);

            var scroller = menu.target.GetComponentDownwards<AbstractScroller>();
            if (scroller is not null)
            {
                scroller.offset = GetScrollLevel(menu);
            }
            
            var layout = xml.SelectFirstComponent<CustomMenuLayout>();
            layout.shouldFixLastItem = false;
            layout.Calculate();
            
            menus.Add(menu);
            return menu;
        }
        
        public CustomMenu CreateMenu(CustomMenuEntry caller)
        {
            var parentMenu = caller.target.GetComponentUpwards<CustomMenu>().target;
            var x = parentMenu.Width() + parentMenu.RelativePosition().x;
            var space = caller.target.EmptySpaceTo(parentMenu);
            var menu = CreateMenu(caller.data.path, new Vector2(x, caller.target.RelativePosition(parentMenu.parent).y - space));
            return menu;
        }
        
        public void ClearMenus(string parentItemPath)
        {
            var toClear = menus.Where(menu => !parentItemPath.Contains(menu.path + "/") && !menu.path.IsNullOrEmpty()).ToList();
            toClear.ForEach(menu => ClearMenu(menu.path));
            menus = menus.Except(toClear).ToList();
        }
        

        public void ClearMenu(string path)
        {
            var menu = menus.First(menu => menu.path == path);
            menu.ClearElements();
            menus.Remove(menu);
        }

        public void ClearMenus()
        {
            menus.ForEach(menu => menu.ClearElements());
            menus = new();
        }

        public void Refresh()
        {
            menuPaths = menus.Select(m => m.path).ToList();
            
            #if UNITY_2022_1_OR_NEWER
            //Assert.IsFalse(menuPaths.IsNullOrEmpty());
            #else
            if (menus.Any(m => m.target.ContainingWindow() is null))
                return; //this window probably leaked
            #endif

            var someData = menus.Select(m => (m.path, m.target.resolvedStyle.left, m.target.resolvedStyle.top));

            ClearMenus();

            for (int i = 0; i < menuPaths.Count; i++)
            {
                var path = menuPaths[i];
                if(typeof(MenuDataSerializer).GetFieldValue<DataNode>("allMenus").Find(path) is not null)
                    CreateMenu(path, new Vector2(someData.FirstOrDefault(d => d.path == path) is {} d ? d.left : 0,
                        someData.FirstOrDefault(d => d.path == path) is {} dd ? dd.top : 0));
            }
            
        }

        public static void StaticRefresh()
        {
            (UnityWindows.GetWindow<SettingsPage>().rootVisualElement.SelectFirstComponent<CustomMenu>().manager as SettingsMenuManager).Refresh();
        }

        public void Refresh2(bool _)
        {
            Refresh();
        }
        
        public void Refresh3(bool newValue)
        {
            menuPaths = menus.Select(m => m.path).ToList();
            if (newValue)
            {
                var second = menuPaths[1];
                ClearMenus();
                CreateMenu(second, Vector2.zero);
            }
            else
            {
                var first = menuPaths[0];
                ClearMenus();
                CreateMenu(first, Vector2.zero);
            }
        }
        
        public void RefreshMenu(string path)
        {
            var oldMenu = menus.First(menu => menu.path == path);
            var newMenu = CreateMenu(path, oldMenu.target.ScreenPosition());
            //(newMenu.element as ScrollView).scrollOffset = (newMenu.element as ScrollView).scrollOffset;
            ClearMenu(path);

        }
        
        public void SetScale(float scale)
        {
            menuContainer.SetScale(scale);
        }

        private void BlockCustomization(VisualElement xml, string path)
        {
            //todo can't block moved menu
            xml.FirstComponent<AbstractScroller>().SetBorder(8, new Color(1,1,0,0.5f));
            
            xml.RegisterCallback<MouseDownEvent>(evt => 
            {
                evt.StopPropagation();
                Debug.Log($"<color=#ffff00>The submenu {path} is managed by Unity and can't be customized</color>");
            }, TrickleDown.TrickleDown);
            
            xml.RegisterCallback<MouseUpEvent>(evt => 
            {
                evt.StopPropagation();
            }, TrickleDown.TrickleDown);
        }

        private float GetScrollLevel(CustomMenu menu)
        {
            return Config.GetSettingOrDefault<float>($"{menu.path}_scroll_level");
        }
    }
}