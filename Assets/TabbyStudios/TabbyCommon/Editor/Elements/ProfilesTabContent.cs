using System.Linq;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class ProfilesTabContent : TabContent
    {
        private static string itemXml= "Item";
        private VisualElement menuView;
        
        private string currentProfile => Config.GetSetting<string>("profile");
        
        
        public override void Start()
        {
            menuView = target.FirstComponent<TabbyScrollView>();
        
            foreach (var profile in TabbyCommonFiles.profiles)
            {
                CreateElement(profile);
            }
        }

        private VisualElement GetElement(string profileName)
        {
            return menuView.Children().Select(c => c.GetComponent<ProfileItem>()).FirstOrDefault(i => i?.profile == profileName)?.target;
        }

        private void CreateElement(string profileName)
        {
            var e = AssetCache.LoadXml(itemXml);
            e.AddComponent<ProfileItem>(profileName);
            menuView.AddElement(e);
        }
    
        private void DeleteElement(string name)
        {
            GetElement(name)?.Pluck();
        }

        public void NewProfile()
        {
            var name = $"New Profile {menuView.Children().Count()}";
            MenuDataSerializer.NewProfile(name);
            CreateElement(name);
        }
    
        public void DeleteProfile(string name)
        {
            if (TabbyCommonFiles.profiles.Count == 1)
                return;
            MenuDataSerializer.DeleteProfile(name);
            DeleteElement(name);
            Config.SetSetting("profile",TabbyCommonFiles.profiles.First());
        }
    
        public void RenameProfile()
        {
            NamePopup.ShowWindow(this,currentProfile,GetElement(currentProfile));
        }

        public void FinishRename(string oldName, string newName)
        {
            DeleteElement(oldName);
            CreateElement(newName);
            MenuDataSerializer.DuplicateCurrentProfile(newName);
            MenuDataSerializer.DeleteProfile(oldName);
            Config.SetSetting("profile",newName);
        }

        public void DuplicateCurrentProfile()
        {
            var newName = $"{currentProfile} Copy {TabbyCommonFiles.profiles.Count}";
            MenuDataSerializer.DuplicateCurrentProfile(newName);
            CreateElement(newName);
        }
    
    }
}