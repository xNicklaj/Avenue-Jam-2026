namespace TabbyStudios
{
    public class TabbyConfig
    {
        [Setting(Profiles.defaultProfile)] 
        private string profile;
    
        [Setting(false)] 
        private static bool lockButton;
        
        [Setting(true)] 
        private static bool expandTopLevelMenus;
        
        [Setting(true)] 
        private static bool multiMonitorFix;
    
        [Setting("WelcomeTab")]
        private string lastOpenTab;
        
        [Setting(false)] 
        private bool previewButton;
    
        [Setting(1f)]
        private float settingsMenuZoom;
        
        [Setting(false)]
        private bool holdKeyToReorder;
    
        [Setting(true)]
        private bool useCustomMenus;
    
        [Setting(true)]
        private bool useIcons;
    
        [Setting("Shift")]
        private string defaultMenuDropdown;
    
        [Setting(280)]
        private int maxMenuWidth;
    
        [Setting(500)]
        private int maxMenuHeight;
        
        [Setting(false)] 
        private bool assetInitialized;
    
        [Setting(125)] 
        private int menuOpenDelay;
    
        [Setting(false)] 
        private bool closeMenusOnAltTab;
    
        [Setting(100)] 
        private int menuScale;
    
        [Setting(100)]
        private int maxItemHeight;
        
        [Setting(false)]
        private bool fallbackInputHandling;
        
        [Setting(false)]
        private bool jsonPrettyPrint;

        [Setting(true)]
        private bool autoDeleteMenuItems;
        
        [Setting(12)]
        private int menuFontSize;

        [Setting(false)]
        public static bool useAnonymousMenus;
    }
}