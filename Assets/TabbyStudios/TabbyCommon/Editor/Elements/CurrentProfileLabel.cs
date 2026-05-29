using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class CurrentProfileLabel : VisualComponent
    {
        private string profileName;
    
        public override void Start()
        {
            profileName = Config.Subscribe<string>("profile",OnProfileChanged);
            SetText();
        }

        private void SetText()
        {
            var label = target as Label;
            label.text = $"Current Profile: {profileName}";
        }

        private void OnProfileChanged(string newName)
        {
            profileName = newName;
            SetText();
        }
    }
}