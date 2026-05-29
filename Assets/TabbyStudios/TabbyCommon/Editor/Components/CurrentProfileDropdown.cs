using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class CurrentProfileDropdown : VisualComponent
    {
        private DropdownField dropdown => (DropdownField)target;
        
        public override void Awake()
        {
            UpdateChoices();
            dropdown.RegisterCallback<ChangeEvent<string>>(OnChange);
            RegisterMouseDown();
        }

        public override void Start()
        {
            UpdateButtons(Profiles.instance.currentProfile);
        }

        public void UpdateChoices()
        {
            dropdown.choices = Profiles.instance.currentProjectProfileNames;
            dropdown.index = dropdown.choices.IndexOf(Profiles.instance.currentProfile);
        }

        private void UpdateButtons(string profile)
        {
            var buttons = target.FindComponent<ProfileButtons>().target;
            var renameProfileButton = buttons.GetComponentDownwards<RenameProfileButton>().target;
            var deleteProfileButton = buttons.GetComponentDownwards<DeleteProfileButton>().target;
            
            if (profile == Profiles.defaultProfile)
            {
                renameProfileButton.SetEnabled(false);
                deleteProfileButton.SetEnabled(false);
            }
            else
            {
                renameProfileButton.SetEnabled(true);
                deleteProfileButton.SetEnabled(true);
            }
            
            UpdateCustomizableStatus(profile);
        }

        public void UpdateCustomizableStatus(string profile)
        {
            var menuCustomizationContainer = target.FindComponent<MenuCustomizationContainer>().target;

            if (profile == Profiles.defaultProfile)
            {
                menuCustomizationContainer.style.opacity = 0.5f;
                foreach (var t in menuCustomizationContainer.SelectComponent<VisualComponent>())
                {
                    if (t is IOpacity o)
                        o.opacity = 0.5f;

                    t.UnregisterMouseDown();
                }
            }
            else
            {
                menuCustomizationContainer.style.opacity = 1;
            }
        }

        public override void OnMouseDown(MouseDownEvent e)
        {
            
        }

        private void OnChange(ChangeEvent<string> e)
        {
            Profiles.instance.ChangeProfile(e.newValue);
            SettingsMenuManager.StaticReset();
            UpdateButtons(e.newValue);
        }

        public override void OnDisable()
        {
            dropdown.UnregisterCallback<ChangeEvent<string>>(OnChange);
        }
    }
}