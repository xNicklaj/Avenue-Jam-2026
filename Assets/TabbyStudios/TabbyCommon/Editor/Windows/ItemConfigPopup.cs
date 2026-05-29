using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TabbyStudios
{
    public class ItemConfigPopup : ContentWrapperPopup<ItemConfigPopup>
    {
        private static TabbyInput input = TabbyInput.Input(1);

        public override string xmlName => "ItemConfig";
        public CustomMenuEntry entry => data as CustomMenuEntry;

        private List<Action> cleaunp = new();

        private bool pickedHasOpened;
        
        public override void ProcessData(VisualElement r)
        {
            r.SelectComponent<ItemDataModifier>().ForEach(em => em.data = entry.data);
            r.Children().ForEach(c => c.SetPadding(1));

            var iconSelector = r.Q<ObjectField>("IconSelector");
            var iconColorSelector = r.Q<ColorField>("IconColorSelector");
            var backgroundColorSelector = r.Q<ColorField>("BackgroundColorSelector");
            var displayNameField = r.Q<TextField>("DisplayNameField");
            
            if (entry is Item item)
            {
                iconSelector.value = IconLoader.GetIcon(entry.data.iconName);
                if (iconSelector.value is null)
                {
                    SetIcon(item, null);
                }

                EventCallback<ChangeEvent<Object>> iconCallback = e => SetIcon(item, (Texture2D)e.newValue);
                iconSelector.RegisterCallback(iconCallback);
                cleaunp.Add(() => iconSelector.UnregisterCallback(iconCallback));

                iconColorSelector.value = UnityColors.IconColor(entry.data.iconColor);

                EventCallback<ChangeEvent<Color>> iconColorCallback = e =>
                {
                    Profiles.instance.menuSerializer.SetProperty(entry.data, i => i.iconColor = e.newValue);
                    item.SetIcon();
                };
                iconColorSelector.RegisterCallback(iconColorCallback);
                cleaunp.Add(() => iconColorSelector.UnregisterCallback(iconColorCallback));

                displayNameField.value = entry.data.displayName;
                EventCallback<ChangeEvent<string>> renameCallback = e =>
                {
                    Profiles.instance.menuSerializer.SetProperty(entry.data, item => item.displayName = e.newValue);
                    item.SetText();
                };
                displayNameField.RegisterCallback(renameCallback);
                cleaunp.Add(() => displayNameField.UnregisterCallback(renameCallback));

                if (entry.data.backgroundColor == new Color())
                {
                    backgroundColorSelector.value = Color.black;
                }
                else
                {
                    backgroundColorSelector.value = entry.data.backgroundColor;
                }

                EventCallback<ChangeEvent<Color>> backgroundColorCallback = e =>
                {
                    Profiles.instance.menuSerializer.SetProperty(entry.data, i => i.backgroundColor = e.newValue);
                    item.SetBackgroundColor();
                };
                backgroundColorSelector.RegisterCallback(backgroundColorCallback);
                cleaunp.Add(() => backgroundColorSelector.UnregisterCallback(backgroundColorCallback));
            }
            else
            {
                r.Q("ItemButtons").Siblings().ForEach(s => s.Hide());
            }
            
            EditorApplication.delayCall += CloseIfRequired;
        }

        private void SetIcon(Item item, Texture2D icon)
        {
            Profiles.instance.menuSerializer.SetProperty(entry.data,i => i.iconName = icon is null ? "" : icon.name);
            item.SetIcon();
        }
        
        public void OnGUI()
        {
            if (input.key == KeyCode.Escape)
            {
                Dispose();
            }
        }
        
        private void CloseIfRequired()
        {
            if (!SettingsPageIsOpen())
            {
                Dispose();
                return;
            }

            EditorApplication.delayCall += CloseIfRequired;
        }

        private bool SettingsPageIsOpen()
        {
            return UnityWindows.GetWindow(TabbyAssets.settingsPage) is { } nn && nn.IsOpen();
        }
        
        public override void OnLostFocus()
        {
            EditorApplication.update += CloseIf;
        }

        private void CloseIf()
        {
            EditorApplication.update -= CloseIf;
            
            if (focusedWindow is null or CustomMenuWindow)
                return;
            
            if (!new[]{"ObjectSelector", "ColorPicker", "SearchPickerWindow"}.Any(focusedWindow.GetType().Name.Contains))
            {
                Dispose();
            }
        }
        
        public void OnEnable()
        {
            var windows = UnityWindows.GetWindows(Window.ObjectSelector).Concat(UnityWindows.GetWindows(Window.ColorPicker));
            foreach (var w in windows)
            {
                //I'm not sure why but some objects pickers from elsewhere can't be closed because parent is null
                if (w.GetPropertyInfo("m_Parent") == null)
                {
                    continue;
                }
                
                if (w.GetPropertyValue("m_Parent") != null)
                {
                    w.Close();
                }
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            foreach (var callback in cleaunp)
            {
                callback();
            }
        }
    }
}