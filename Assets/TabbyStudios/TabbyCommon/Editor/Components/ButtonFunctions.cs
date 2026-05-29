using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace TabbyStudios
{

    public class ViewChangelogButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string url = "https://assetstore.unity.com/packages/tools/utilities/tabby-context-enhanced-context-menus-311600#releases";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
    
    public class DiscordButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string url = "https://discord.gg/fBX3yAs8BG";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
    
    public class MoreByTabbyStudiosButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string url = "https://assetstore.unity.com/publishers/113033";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
    
    public class ImportExamplesButton : ButtonComponent
    {
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            var tabbyContextExamples = TabbyAssets.tabbyContextFiles?.GetMemberValue<string>("examplesPath");
            var tabbyMenusExamples = TabbyAssets.tabbyMenusFiles?.GetMemberValue<string>("examplesPath");
        
            var files = new List<string>();
            
            if(tabbyContextExamples is not null)
                files.AddRange(Directory.GetFiles(tabbyContextExamples));
            
            if(tabbyMenusExamples is not null)
                files.AddRange(Directory.GetFiles(tabbyMenusExamples));
        
            
            foreach (var file in files)
            {
                if (!file.EndsWith(".json"))
                    continue;
                
                Profiles.instance.menuSerializer.Import(file);
                target.FindComponent<CurrentProfileDropdown>()?.UpdateChoices();
                SettingsMenuManager.StaticRefresh();
            }
            
            Debug.Log("Example profiles imported");
        }
    }
    
    public class RebuildDefaultProfileButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            Profiles.instance.menuSerializer.LoadDefaultProfile();
            target.FindComponent<CurrentProfileDropdown>()?.UpdateChoices();
            SettingsMenuManager.StaticRefresh();
        }
    }

    public class ImportProfileButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string path = EditorUtility.OpenFilePanel("Select profile", "Assets", "json");
            if (!path.IsNullOrEmpty())
            {
                Profiles.instance.menuSerializer.Import(path);
                target.FindComponent<CurrentProfileDropdown>().UpdateChoices();
                SettingsMenuManager.StaticRefresh();
            }
        }
    }
    
    public class ExportProfileButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            Profiles.instance.Export(Profiles.instance.currentProfile, "Assets");
        }
    }
    
    public class NewProfileButton : ButtonComponent
    {
        public override void Awake()
        {
            base.Awake();
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            var pattern = @"^New Profile\s*(\d+)?$";
            var matches = Profiles.instance.currentProjectProfileNames.Count(p => Regex.IsMatch(p, pattern));
            var name = $"New Profile {matches + 1}";
            Profiles.instance.CreateProfile(name);
            Profiles.instance.ChangeProfile(name);
            target.FindComponent<CurrentProfileDropdown>().UpdateChoices();
        }
    }

    public class DeleteProfileButton : IconButton
    {
        public override void Awake()
        {
            base.Awake();
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            var name = Profiles.instance.currentProfile;
            Profiles.instance.DeleteProfile(name);
            target.FindComponent<CurrentProfileDropdown>().UpdateChoices();
        }
    }

    public class DuplicateProfileButton : ButtonComponent
    {
        public override void Awake()
        {
            base.Awake();
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }

        public override void OnMouseUp(MouseUpEvent e)
        {
            var name = Profiles.instance.GetUniqueName(Profiles.instance.currentProfile);
            Profiles.instance.DuplicateCurrentProfile(name);
            Profiles.instance.ChangeProfile(name);
            target.FindComponent<CurrentProfileDropdown>().UpdateChoices();
        }
    }
    
    public class ResetToDefaultButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            ResetPreferencesPopup.Create(MouseUtil.MousePosition(e));
        }
    }

    public class ExportSettingsButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            Config.instance.Export("Assets");
        }
    }

    public class ImportSettingsButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string path = EditorUtility.OpenFilePanel("Select config file", "Assets", "json");
            if (!path.IsNullOrEmpty())
            {
                TabbyAssets.DisposeSettingsPage(); //close the settings because this will trigger unwanted subscribers
                Config.instance.Import(path);
            }
        }
    }
    
    public class DeleteItemButton : ButtonComponent, ItemDataModifier
    {
        public ItemData data { get; set; }

        public override void OnMouseUp(MouseUpEvent e)
        {
            DeleteItemPopupWindow.Create(MouseUtil.MousePosition(e), data).Display();
        }
    }
    
    public class AssetStoreButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            string url = "https://u3d.as/3u2C";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
    
    public class DocumentationButton : ButtonComponent
    {
        public override void OnMouseUp(MouseUpEvent e)
        {
            if (TabbyAssets.hasTabbyContext && TabbyAssets.tabbyContextFiles.GetMemberValue("documentationPath") is string contextPath && File.Exists(contextPath)) 
            {
                var dir = Directory.GetCurrentDirectory();
                contextPath = Path.Combine(dir, contextPath);
                Process.Start(new ProcessStartInfo(contextPath) { UseShellExecute = true });
            }
            
            if (TabbyAssets.hasTabbyMenus && TabbyAssets.tabbyMenusFiles.GetMemberValue("documentationPath") is string menusPath && File.Exists(menusPath)) 
            {
                var dir = Directory.GetCurrentDirectory();
                contextPath = Path.Combine(dir, menusPath);
                Process.Start(new ProcessStartInfo(contextPath) { UseShellExecute = true });
            }
        }
    }
    
    public class RenameProfileButton : ButtonComponent
    {
        public override void Awake()
        {
            base.Awake();
            target.style.unityBackgroundImageTintColor = UnityColors.textColor;
        }
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            NamePopup.ShowWindow(FinishRename, Profiles.instance.currentProfile, target.ScreenPosition());
        }

        public void FinishRename(string oldName, string newName)
        {
            Profiles.instance.DuplicateCurrentProfile(newName);
            Profiles.instance.ChangeProfile(newName);
            Profiles.instance.DeleteProfile(oldName);
            target.FindComponent<CurrentProfileDropdown>().UpdateChoices();
        }
    }
    
    public class MenuToolbarButton : VisualComponent
    {
        public string menuPath;
        
        public override void OnMouseUp(MouseUpEvent e)
        {
            var menu = target.FindComponent<CustomMenu>();
            var manager = (SettingsMenuManager)menu.manager;
            var button = (Button)target;
            Config.instance.Set(nameof(ShowMenuToolbar.lastShowMenuInMenusTab), menuPath);
            button.AddToClassList("selected");
            manager.ClearMenus();
            manager.CreateMenu(menuPath, Vector2.zero);
            
            var newMenu = target.FindComponent<CustomMenu>();
            newMenu.target.FindComponent<CurrentProfileDropdown>().UpdateCustomizableStatus(Profiles.instance.currentProfile);
        }
    }
    
}