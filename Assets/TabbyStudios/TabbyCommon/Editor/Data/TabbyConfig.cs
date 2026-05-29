namespace TabbyStudios
{
    public static class TabbyConfig
    {
        [Setting(Profiles.defaultProfile)] 
        public static string profile;
    
        [Setting(false)] 
        public static bool lockButton;
        
        [Setting(true)] 
        public static bool expandTopLevelMenus;
        
        [Setting("WelcomeTab")]
        public static string lastOpenTab;
        
        [Setting(false)] 
        public static bool previewButton;
    
        [Setting(1f)]
        public static float settingsMenuZoom;
        
        [Setting(false)]
        public static bool holdKeyToReorder;
    
        [Setting(true)]
        public static bool useCustomMenus;
    
        [Setting(true)]
        public static bool useIcons;
    
        [Setting("Shift")]
        public static string defaultMenuDropdown;
    
        [Setting(280)]
        public static int maxMenuWidth;
    
        [Setting(500)]
        public static int maxMenuHeight;
        
        [Setting(false)] 
        public static bool assetInitialized;
    
        [Setting(125)] 
        public static int menuOpenDelay;
    
        [Setting(true)] 
        public static bool closeMenusOnAltTab;
    
        [Setting(100)] 
        public static int menuScale;
    
        [Setting(100)]
        public static int maxItemHeight;
        
        [Setting(false)]
        public static bool fallbackInputHandling;

        [Setting(true)]
        public static bool autoDeleteMenuItems;
        
        [Setting(12)]
        public static int menuFontSize;

        [Setting(false)]
        public static bool useAnonymousMenus;

        [Setting(false)]
        public static bool menuFadein;

        [Setting(false)]
        public static bool roundedMenuCorners;

        [Setting(-1)]
        public static int maxItemsPerMenu;

        public static string ScrollLevel(string menu)
        {
            return $"{menu}_scroll_level";
        }
    }
}