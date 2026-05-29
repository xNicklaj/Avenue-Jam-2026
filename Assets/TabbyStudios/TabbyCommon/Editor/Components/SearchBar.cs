using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class SearchBar : VisualComponent
    {
        [Setting(true)]
        public static bool useSearchBar;
    
        [Setting(true)]
        public static bool useSearchBarOnSubmenus;
        
        [Setting(false)]
        public static bool alwaysShowSearchBar;

        public bool visible { get; private set; }
        public bool enabled { get; private set; }

        private ToolbarSearchField searchBar => target.Q<ToolbarSearchField>();
    
        public override void Awake()
        {
            searchBar.RegisterCallback<ChangeEvent<string>>(OnChange);
            searchBar.RegisterCallback<KeyDownEvent>(OnKeyDown,TrickleDown.TrickleDown);
        }

        public void OnChange(ChangeEvent<string> e)
        {
            if (!IsEnabled())
                return;
            
            var menu = target.GetComponentUpwards<CustomMenu>().target.SelectFirstComponent<CustomMenu>();
            menu.target.RemoveComponent(menu.provider);
            var provider = menu.target.AddComponent(NewItemProvider(e.newValue));
            menu.provider = (ItemProvider)provider;
            menu.Refresh();

            if (e.newValue.IsNullOrEmpty())
            {
                if (!Config.instance.GetBool(nameof(alwaysShowSearchBar)))
                {
                    HideBar();
                }
            }
            else
            {
                ShowBar();
            }
            
            menu.target.Root().GetComponentDownwards<CustomMenuLayout>().Calculate(menu.target.resolvedStyle.width, menu.target.resolvedStyle.height);
            menu.manager.ClearMenus($"{menu.path}/");
        }

        public void EnableBar()
        {
            enabled = true;
        }

        public void DisableBar()
        {
            enabled = false;
        }

        public void ShowBar()
        {
            visible = true;
            Show();
        }

        public void HideBar()
        {
            visible = false;
            Hide();
        }

        public void CharacterTypedWithoutFocus(KeyCode key)
        {
            if (!IsEnabled())
                return;
            
            if (target.IsFocused())
                return;
            
            searchBar.value = key.ToString();
            searchBar.Focus();
        }
    
        private new void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode is KeyCode.UpArrow or KeyCode.DownArrow)
            {
                e.StopPropagation();
                ((CustomMenuWindow)target.ContainingWindow()).TryHandleArrows(e.keyCode); //this is bad
            }

            else if (e.keyCode is KeyCode.Return or KeyCode.KeypadEnter)
            {
                var menu = target.GetComponentUpwards<CustomMenu>();
                menu.SelectCurrent();
            }
        }
    
        public bool IsEnabled()
        {
            return enabled;
        }

        private ItemProvider NewItemProvider(string search)
        {
            if (search.IsNullOrEmpty())
                return new EditorItemProvider {disableCollapsingSeparators = true};

            return new SearchItemProvider(search);
        }
    
    }
}