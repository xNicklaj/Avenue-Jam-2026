using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class NamePopup : TabbyWindow<NamePopup>
    {
        public static void ShowWindow(Action<string, string> finishRename, string oldName, Vector2 pos)
        {
            var window = CreateInstance<NamePopup>();
            windows.Add(window);
            
            window.defaultMode = Mode.Popup;
            var content = AssetCache.LoadXml("NamePopup");
            window.rootVisualElement.Add(content);
            var textField = content.Q<TextField>();
            content.Q<Button>().clicked += () =>
            {
                if (IsValidName(textField.text))
                {
                    window.Dispose();
                    finishRename(oldName, textField.text);
                }
                else
                {
                    Debug.Log("<color=#ffff00>Use a name not containing . or _ that's not a reserved file name</color>");
                }
            };
        
            window.Display();
            window.position = new Rect(pos.AddY(26), new Vector2(200, 70));
            window.Focus();
        }

        public static bool IsValidName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            char[] invalidChars = Path.GetInvalidFileNameChars();
            
            if (fileName.IndexOfAny(invalidChars) >= 0)
                return false;
            
            if (fileName.Contains(".") || fileName.Contains("_"))
                return false;
            
            if (fileName.Contains('\0') || fileName.Contains('/'))
                return false;
            
            string[] reservedNames =
            {
                "CON", "PRN", "AUX", "NUL",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName).ToUpper();
            if (reservedNames.Contains(nameWithoutExtension))
                return false;

            if (fileName.EndsWith(".") || fileName.EndsWith(" "))
                return false;
            
            if (System.Text.Encoding.UTF8.GetByteCount(fileName) > 255)
                return false;

            return true;
        }
        
        public override void OnLostFocus()
        {
            Dispose();
        }
    }
}