using System;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public static class UnityColors
    {

        public static Func<bool> darkModeFunc = () => EditorGUIUtility.isProSkin;
        public static bool darkMode => darkModeFunc();
    
        public static Color defaultHoveredColor => (darkMode ? new Color(70, 96, 124, 255) : new Color(150, 195, 251, 255)) / 255f;
        public static Color textColor => (darkMode ? new Color(210, 210, 210, 255) : new Color(18, 18, 18, 255)) / 255f;
        public static Color tabChecked => (darkMode ? new Color(52, 52, 52, 255) : new Color(150, 150, 150, 255)) / 255f;
        public static Color randomBorder => (darkMode ? new Color(45, 45, 45, 255) : new Color(155, 155, 155, 255)) / 255f;
        public static Color tabHovered => (darkMode ? new Color(65, 65, 65, 255) : new Color(180, 180, 180, 255)) / 255f;
        public static Color defaultColor => (darkMode ? new Color(57, 57, 57, 255) : new Color(200, 200, 200, 255)) / 255f;
        
        public static Color IconColor(Color iconColor)
        {
            return iconColor == default ? textColor : iconColor;
        }
        
        public static Color ItemHoverColor(Color backgroundColor)
        {
            return backgroundColor == default ? defaultHoveredColor : backgroundColor*0.75f;
        }
        
    }
}