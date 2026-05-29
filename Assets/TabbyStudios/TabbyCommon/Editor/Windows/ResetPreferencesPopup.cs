using UnityEditor.Compilation;

namespace TabbyStudios
{
    public class ResetPreferencesPopup : ConfirmationPopup<ResetPreferencesPopup>
    {
        public override void Confirm()
        {
            UnityWindows.GetWindow<SettingsPage>().Dispose();
            
            EditorUtil.DelayCall(() =>
            {
                Config.NewConfig();
                Config.SetSetting("lastOpenTab", "PreferencesTab");
                CompilationPipeline.RequestScriptCompilation();
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