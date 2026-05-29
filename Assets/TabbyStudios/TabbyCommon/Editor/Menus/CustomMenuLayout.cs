using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class CustomMenuLayout : VisualComponent
    {
        private static float startingIconSize = 24f;
        private static float startingItemHeight = 26f;
        
        public static float iconSize = 24;
        public static float arrowWidth = 20;
        public static float iconMargin = 1;
        public static float menuBorder = 1;
        public static float menuPadding = 5;
        public static float itemHeight = 26;
        public static float separatorHeight = 6;
        public static float searchBarHeight = 34;
        public static float extraSpace = 12;
        
        private static int maxWidth => Config.GetSetting<int>("maxMenuWidth");
        private static int maxHeight => Config.GetSetting<int>("maxMenuHeight");
        private float totalSearchHeight;
        private TabbyScrollView scroller;
        private bool hasSearchBar;
        
        public Vector2 calculatedSize { get; private set; }
        public bool shouldFixLastItem = true;
        
        private static float widthThing = iconSize + arrowWidth + 2*(iconMargin + menuBorder + menuPadding) + extraSpace;

        static CustomMenuLayout()
        {
            Config.Subscribe<int>("maxItemHeight", CalculateItemHeight, callImmediately:true);
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
        
        public void Calculate(float fixedWidth, float fixedHeight)
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
                 if (nameLabel is null)
                 {
                     continue;
                 }
                 
                 var textComp = nameLabel.GetComponent<TextComponent>();
                 var width = textComp.TextSize().x;
                 nameLabel.style.width = width;
                 
                 var shortcut = item.Query().Build().FirstOrDefault(e => e.Name() == "Shortcut");
                 if (shortcut is null)
                 {  
                     continue;
                 }
                 
                 var shortcutText = shortcut.GetComponent<TextComponent>();
                 var shortcutWidth = shortcutText.TextSize().x;
                 shortcut.style.width = shortcutWidth;
                 
                 nameLabel.style.maxWidth = maxWidth - widthThing - shortcutWidth;

                 
                 if (width + shortcutWidth > maxName)
                 {
                     maxName = width + shortcutWidth;
                 }
             }
             
             // ReSharper disable once CompareOfFloatsByEqualityOperator
             if (fixedWidth == -1)
             {
                 CalculateSize(maxName);
             }
             else
             {
                 CalculateSizeWithSize(fixedWidth, fixedHeight);
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

             GrowLabelsToPreventClipping(scroller);
        }

        private void CalculateSizeWithSize(float width, float height)
        {
            totalSearchHeight = hasSearchBar ? searchBarHeight : 0;
            calculatedSize = new Vector2(Mathf.Min(width, maxWidth), Mathf.Min(height + totalSearchHeight, maxHeight));
            //todo maybe grow the window a bit if its too small
        }
        
        private void CalculateSize(float maxName)
        {
            var width = maxName + widthThing;
            var height = scroller.items.Sum(ItemHeight) + 2*(menuPadding+menuBorder);
            CalculateSizeWithSize(width, height);
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

        private void GrowLabelsToPreventClipping(TabbyScrollView scroller)
        {
            for (int i = 0; i < scroller.items.Count; i++)
            {
                var nameLabel = scroller.items[i].Query().Build().FirstOrDefault(e => e.Name() == "Name");
                if (nameLabel is null) continue;
                nameLabel.style.width = nameLabel.style.width.value.value + Mathf.Min(nameLabel.style.maxWidth.value.value - nameLabel.style.width.value.value, 10);
            }
        }
    }
}