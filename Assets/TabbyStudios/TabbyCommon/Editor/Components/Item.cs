using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class Item : CustomMenuEntry
    {
        public static string xml = "Item";
        private TextComponent nameLabel;
        private float textLength,containerLength;
        private bool shouldAnimate;
        private float displacement => containerLength - textLength;
        public bool lockColor;
        
        private static List<(string, string)> pairs = new ()
        {
            ("%", "Ctrl"),
            ("^", "Ctrl"),
            ("#", "Shift"),
            ("&", "Alt"),
            ("_DEL", "Del"),
            ("_PGUP", "PgUp"),
            ("_BACKSPACE", "Backspace"),
            ("_TAB", "Tab"),
            ("_HOME", "Home"),
            ("_PGDN", "PgDn"),
            ("_END", "End"),
            ("_INS", "Insert"),
            ("_", ""),
        };
        
        public Item(ItemData data) : base(data)
        {
        }

        public override VisualElement CreateParent()
        {
            return AssetCache.LoadXml(xml);
        }
        
        public override void Awake()
        {
            RegisterMouseEnter();
            RegisterMouseLeave();

            var fontSize = Config.instance.GetInt(nameof(TabbyConfig.menuFontSize));
            
            nameLabel = target.SelectFirstComponent<TextComponent>();
            if (nameLabel is not null)
            {
                nameLabel.text = data.displayName;
                var comp = nameLabel.GetComponent<TextComponent>();
                comp.fontColor = UnityColors.textColor;
                comp.fontSize = fontSize;
            }
            
            var shortcutLabel = target.Query().Build().First(t => t.Name() == "Shortcut");
            var shortcutText = GetShortCut(data.originalPath);
            if (shortcutText != "")
            {
                var comp = shortcutLabel.GetComponent<TextComponent>();
                comp.text = shortcutText;
                comp.fontSize = fontSize - 3;
                comp.fontColor = UnityColors.textColor;
            }
            else
            {   
                shortcutLabel.style.visibility = Visibility.Hidden;
                shortcutLabel.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
            }

            var arrow = target.Q("ArrowContainer");
            arrow.style.unityBackgroundImageTintColor = UnityColors.textColor;
            if (!data.hasChildren)
            {
                arrow.style.visibility = Visibility.Hidden;
            }

            SetGradient();
            SetBackgroundColor();
            SetIcon();
            SetSpacing();

        }

        public void SetText()
        {
            nameLabel.text = data.displayName;
            nameLabel.target.MarkDirtyRepaint();
        }
        
        public void SetIcon()
        {
            if (!Config.instance.GetBool("useIcons")) 
                return;

            var icon = target.Q("Icon");

            icon.style.height = CustomMenuLayout.iconSize;
            icon.style.width = CustomMenuLayout.iconSize;

            if (!data.iconName.IsNullOrEmpty())
            {
                icon.style.backgroundImage = IconLoader.GetIcon(data.iconName);
                icon.style.unityBackgroundImageTintColor = UnityColors.IconColor(data.iconColor);
            }
            else
            {
                icon.style.backgroundImage = null;
            }

        }

        public void SetBackgroundColor()
        {
            target.SetColor(data.backgroundColor);
        }

        public void SetGradient()
        {
            if (data.useGradient)
                target.style.backgroundImage = IconLoader.gradient;
        }

        public void SetSpacing()
        {
            target.style.height = CustomMenuLayout.itemHeight;
        }
    
        public string GetShortCut(string path)
        {
            #if !UNITY_2023_1_OR_NEWER
                if(shortcuts.IsNullOrEmpty())
                    LoadShortcuts();
                string hotkey = shortcuts.GetOrDefault(path, "");
            #else
                string hotkey = typeof(Menu).InvokeStaticMethod<string>("GetHotkey", TabbyAssets.MapToUnityPath(path));
            #endif
            
            hotkey = hotkey.ToUpper();
            foreach (var pair in pairs)
            {
                hotkey = hotkey.ReplaceMultiple(pair.Item1, $"{pair.Item2}+");
            }

            hotkey = hotkey.RemoveTrailing("+").RemoveLeading("+");
            return hotkey;
        }

        #if !UNITY_2023_1_OR_NEWER
        private static Map<string, string> shortcuts = new();
        private static void LoadShortcuts()
        {
            var hotkeys = new List<string>();
            var paths = new List<string>();
            typeof(Menu).InvokeStaticMethod<string>("GetMenuItemDefaultShortcuts", paths, hotkeys);
            shortcuts = new(paths.Select((p, i) => (p, hotkeys[i])));
        }
        #endif
        
        private VisualElement UpwardsName(VisualElement e, string name)
        {
            return e.name == name ? e : e.parent is null ? null : UpwardsName(e.parent, name);
        }

        private void TryAnimate()
        {
            if (!shouldAnimate)
                return;
            //nameLabel.Animate("translate",Mathf.Abs(displacement/75f),0.5f);
        }
    
        private void TryMove()
        {
            if (!shouldAnimate)
                return;
            
            //nameLabel.SetTextPosition(new Vector2(displacement, 0));
        }

        public override void OnMouseEnter(MouseEnterEvent e)
        {
            Select();
        }

        public override void OnMouseLeave(MouseLeaveEvent e)
        {
            Deselect();
        }

        public virtual void Select()
        {
            selected = true;
            target.FindComponent<RadioManager<CustomMenuEntry>>().UncheckOthers(this);
            target.FindComponent<TabbyScrollView>().EnsureItemInView(target);
            target.SetColor(UnityColors.ItemHoverColor(data.backgroundColor));
            TryAnimate();
            TryMove();
        }
    
        public virtual void Deselect()
        {
            selected = false;
            
            if(!lockColor)
                target.SetColor(data.backgroundColor);
            
            //items with no display still have components, spaghetti
            // if (nameLabel is null)
            //     return;
            
            // nameLabel.Animate("translate");
            // nameLabel.SetTextPosition(Vector2.zero);
        }
        
        private static object a = new object[] { }.FirstOrDefault(); //stop rider from removing linq on remove unused imports
    }
}