namespace TabbyStudios
{
    public class ResetPreferencesPopup : ConfirmationPopup<ResetPreferencesPopup>
    {
        public override void Confirm()
        {
            TabbyAssets.DisposeSettingsPage();
            
            EditorUtil.DelayCall(() =>
            {
                Config.instance.ResetDefaults();
                Config.instance.Set(nameof(TabbyConfig.lastOpenTab), "PreferencesTab");
            });
        }

        public override string ConfirmationText()
        {
            return $"Are you sure you want to reset preferences?";
        }

        public override string WarningText()
        {
            return "This can't be undone";
        }
    }
}