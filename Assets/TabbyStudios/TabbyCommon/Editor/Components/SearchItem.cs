using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class SearchItem : CustomMenuEntry
    {
        public static string xml = "SearchItem";

        private InnerItem item;
        private VisualElement itemRoot;
    
        public SearchItem(ItemData data) : base(data)
        {
        
        }
    
        public override VisualElement CreateParent()
        {
            var root = AssetCache.LoadXml(xml);
            item = new InnerItem(data);

            itemRoot = item.CreateParent();
            itemRoot.AddComponent(item);
            root.AddElement(itemRoot);

            return root;
        }

        public override void Start()
        {
            //itemRoot.SetColor(new Color(itemRoot.Root().WhereComponent<SearchItem>().Count/30f,0,0,1));
        }
        
        public override void Awake()
        {
            RegisterGeometryChanged();
            RegisterMouseEnter();
            RegisterMouseLeave();
            itemRoot.SelectFirstComponent<TextComponent>().AdjustSize();
            
            var pathLabel = target.Q<Label>("PathLabel");
            pathLabel.text = PathLabelText(); 
            pathLabel.style.fontSize = 10;
        }

        private string PathLabelText()
        {
            //this is the old version with the root menu name at the start
            //return item.data.path.RemoveLeading(TabbyAssets.extraMenuPrefix).RemoveLeading(TabbyAssets.anonymousMenuPath).RemoveLeading("/");
            return item.data.path.RemoveBeforeFirst("/");
        }

        public override void OnGeometryChanged(GeometryChangedEvent e)
        {
            //itemRoot.SelectFirstComponent<TextComponent>().text = data.displayName;
        }
        
        public override void OnMouseEnter(MouseEnterEvent e)
        {
            item.Select();
        }

        public override void OnMouseLeave(MouseLeaveEvent e)
        {
            item.Deselect();
        }

        public virtual void Select()
        {
            selected = true;
            target.FindComponent<TabbyScrollView>().EnsureItemInView(target);
            target.FindComponent<RadioManager<CustomMenuEntry>>().UncheckOthers(this);
            item.Select();
        }
    
        public virtual void Deselect()
        {
            selected = false;
            item.Deselect();
        }

        private class InnerItem : Item
        {
            public InnerItem(ItemData data) : base(data)
            {
            
            }
            
            public override void Select()
            {
                target.parent.SetColor(UnityColors.ItemHoverColor(data.backgroundColor));
            }
    
            public override void Deselect()
            {
                target.parent.SetColor(data.backgroundColor);
            }

            public override void OnMouseEnter(MouseEnterEvent e)
            {
                
            }

            public override void OnMouseLeave(MouseLeaveEvent e)
            {
                
            }
        }

    }
}