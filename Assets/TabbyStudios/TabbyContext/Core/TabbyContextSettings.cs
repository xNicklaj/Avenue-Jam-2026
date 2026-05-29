using UnityEditor;

namespace TabbyStudios
{
    public static class TabbyContextSettings
    {
        [MenuItem("Tools/Tabby Context")]
        public static void ShowSettingsPageMenuItem()
        {
            if (UnityWindows.GetWindow<SettingsPage>() is { } w)
            {
                w.Focus();
            }
            else
            {
                SettingsPage.Launch();
            }
        }
    }
}