using System.Collections.Generic;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ShowMenuToolbar : VisualComponent
    {
        [Setting("")]
        public string lastShowMenuInMenusTab;

        private List<string> menuPaths;
        private RadioManager<VisualElement> parentToolBar => target.GetComponentUpwards<MenuToolbarContainer>();
        
        public ShowMenuToolbar(List<string> menuPaths)
        {
            this.menuPaths = menuPaths;
        }
        
        public override void Awake()
        {
            foreach (var menuPath in menuPaths)
            {
                AddEntry(menuPath, menuPath.RemoveLeading(TabbyAssets.extraMenuPrefix));
            }
        }

        private void AddEntry(string menuPath, string text)
        {
            var e = AssetCache.LoadXml("MenuToolbarButton").As<Button>();
            var comp = e.GetComponent<MenuToolbarButton>();
            comp.menuPath = menuPath;

            e.clicked += () => comp.OnMouseUp(null);

            e.RegisterCallback<MouseUpEvent>(OnToggleClick);

            e.text = text;
            var lastMenu = Config.GetSetting<string>(nameof(lastShowMenuInMenusTab));
            if (menuPath == lastMenu)
            {
                e.AddToClassList("selected");
            }
            else if (lastMenu.IsNullOrEmpty())
            {
                e.AddToClassList("selected");
                Config.SetSetting(nameof(lastShowMenuInMenusTab), menuPath);
            }
            target.AddElement(e);
        }
    
        public void OnToggleClick(MouseUpEvent e)
        {
            if (e.button != 0) return;
            parentToolBar.UncheckOthers(e.target as VisualElement);
        }
    }
}