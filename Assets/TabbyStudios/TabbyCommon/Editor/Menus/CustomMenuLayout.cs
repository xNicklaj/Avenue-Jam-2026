using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class CustomMenuLayout : VisualComponent
    {
        private static float startingIconSize = 22f; // unchanged
        private static float startingItemHeight = 24f; // unchanged

        public static float iconSize = 20; // was 24
        public static float arrowWidth = 16; // was 20
        public static float iconMargin = 0; // was 1 (top/bottom margins removed)
        public static float menuBorder = 0.5f; // was 1
        public static float menuPadding = 4; // was 5 (changed for TabbyScrollView top/bottom, but left/right is 0 now)
        public static float itemHeight = 22; // was 26
        public static float separatorHeight = 9; // was 6 (ItemWrapper margins: 4+4=8, plus 1px line = 9 total)
        public static float searchBarHeight = 34; // unchanged (didn't modify)
        public static float extraSpace = 6; // unchanged (not sure what this maps to)
        
        private static int maxWidth => Config.instance.GetInt("maxMenuWidth");
        private static int maxHeight => Config.instance.GetInt("maxMenuHeight"); 
        
        
        private float totalSearchHeight;
        private TabbyScrollView scroller;
        private bool hasSearchBar;
        
        public Vector2 calculatedSize { get; private set; }
        
        private static float widthThing = iconSize + arrowWidth + 2*(iconMargin + menuBorder + menuPadding) + extraSpace;

        static CustomMenuLayout()
        {
            Config.instance.Subscribe<int>(nameof(TabbyConfig.maxItemHeight), CalculateItemHeight);
            CalculateItemHeight(Config.instance.GetInt(nameof(TabbyConfig.maxItemHeight)));
        }

        private static void CalculateItemHeight(int mul)
        {
            iconSize = startingIconSize*mul/100f;
            itemHeight = startingItemHeight*mul/100f;
        }
        
        public void Calculate()
        {
            Calculate(-1, -1);
        }
        
        public void Calculate(float constantWidth, float constantHeight)
        {
             scroller = target.SelectFirstComponent<TabbyScrollView>();
             hasSearchBar = target.GetComponentDownwards<SearchBar>() is { visible: true };

             float maxName = 0;
             
             for (int i = 0; i < scroller.items.Count; i++)
             {
                 var item = scroller.items[i];

                 if(item.style.display == DisplayStyle.None)
                     continue;

                 var nameLabel = item.Query().Build().FirstOrDefault(e => e.Name() == "Name");
                 if (nameLabel is null) continue;
                 
                 var textComp = nameLabel.GetComponent<TextComponent>();
                 var width = textComp.TextSize().x;
                 nameLabel.style.width = width;
                 
                 var shortcut = item.Query().Build().FirstOrDefault(e => e.Name() == "Shortcut");
                 if (shortcut is not null)
                 {
                     var shortcutTextComponent = shortcut.GetComponent<TextComponent>();

                     var shortcutWidth = shortcutTextComponent.TextSize().x;
                     shortcut.style.width = shortcutWidth;

                     nameLabel.style.maxWidth = maxWidth - widthThing - shortcutWidth;

                     if (width + shortcutWidth > maxName)
                     {
                         maxName = width + shortcutWidth;
                     }
                 }
                 else
                 {
                     nameLabel.style.maxWidth = maxWidth - widthThing;

                     if (width > maxName)
                     {
                         maxName = width;
                     }
                 }

             }
             
             // ReSharper disable once CompareOfFloatsByEqualityOperator
             if (constantWidth == -1)
             {
                 CalculateSize(maxName);
             }
             else
             {
                 CalculateSizeWithSize(constantWidth, constantHeight, false);
             }
             
             target.style.width = calculatedSize.x;
             target.style.height = calculatedSize.y;
             
             if (hasSearchBar)
             {
                 scroller.target.style.height = calculatedSize.y - totalSearchHeight;
             }
             else
             {
                 scroller.target.style.height = calculatedSize.y;
             }

             //GrowLabelsToPreventClipping(scroller);
        }

        private void CalculateSizeWithSize(float width, float height, bool fixLastItem)
        {
            totalSearchHeight = hasSearchBar ? searchBarHeight : 0;
            calculatedSize = new Vector2(Mathf.Min(width, maxWidth), Mathf.Min(height + totalSearchHeight, maxHeight));

            if (fixLastItem)
            {
                float totalHeight = 0;
                foreach (var item in scroller.items)
                {
                    var itemHeight = ItemHeight(item);
                    totalHeight += itemHeight;
                    if (totalHeight > maxHeight - totalSearchHeight - 2*(menuPadding + menuBorder))
                    {
                        calculatedSize = new Vector2(Mathf.Min(width, maxWidth), Mathf.Min(totalHeight - itemHeight + totalSearchHeight + 2*(menuPadding + menuBorder), maxHeight));
                        break;
                    }
                }
            }
        }
        
        private void CalculateSize(float maxName)
        {
            var width = maxName + widthThing;
            var height = scroller.items.Sum(ItemHeight) + 2*(menuPadding+menuBorder);
            CalculateSizeWithSize(width, height, true);
        }
        
        
        private float GoodSeparatorHeight(Separator separator)
        {
            var value = separator.target.Children().First().style.height.value.value;
            return float.IsNaN(value) || value == 0 ? separatorHeight : value + 4;
        }

        private float ItemHeight(VisualElement item)
        {
            if (item.style.display == DisplayStyle.None) 
                return 0;
            return item.GetComponent<Separator>() is { } s ? GoodSeparatorHeight(s) : itemHeight;
        }
    }
}