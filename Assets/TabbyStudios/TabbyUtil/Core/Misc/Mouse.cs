#if UNITY_EDITOR_WIN

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public class Mouse
    {
        //Move = 0x00000001
        //Absolute = 0x00008000

        public const float defaultSpeed = 10000;
        
        private static int DownMap(int i) => (int)Mathf.Pow(2, 2*i + 1);
        private static int UpMap  (int i) => (int)Mathf.Pow(2, 2*i + 2);

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetPosition(int x, int y)
        {
            SetCursorPos(x, y);
            //mouse_event(1, x, y, 0, 0);
        }
        
        public static void Scroll(int wheelTicks)
        {
            mouse_event(0x0800, 0, 0, 120*wheelTicks, 0);
        }

        public static void SetPosition(Vector2 pos)
        {
            SetCursorPos((int)pos.x, (int)pos.y);
        }

        public static IEnumerator SetPositionSlowly(Vector2 target, float pixelsPerSecond = defaultSpeed)
        {
            var startPos = new Vector2(GetPosition().x, GetPosition().y);
            var dist = (target - startPos).magnitude;
            
            var totalTime = dist / pixelsPerSecond;
            double elapsed = 0;
            double last = EditorApplication.timeSinceStartup;
            
            while(elapsed <= totalTime)
            {
                elapsed += EditorApplication.timeSinceStartup - last;
                last = EditorApplication.timeSinceStartup;
                
                var current = (float)(1 - elapsed/totalTime)*startPos + (float)(elapsed/totalTime)*target;
                SetPosition(current);

                yield return null;
            }
            
            SetPosition(target);
            yield return null;
        }
        
        public static IEnumerator SetPositionSlowly(int x, int y , float pixelsPerSecond = 10000)
        {
            yield return SetPositionSlowly(new Vector2(x, y), pixelsPerSecond);
        }


        public static void MouseEvent(int value)
        {
            mouse_event(value, 0, 0, 0, 0);
        }

        public static IEnumerator Click(int button)
        {
            MouseEvent(DownMap(button));
            yield return null;
            MouseEvent(UpMap(button));
        }

        public static IEnumerator Click(int button, int x, int y)
        {
            SetPosition(x,y);
            yield return Click(button);
        }

        public static IEnumerator Click(int button, Vector2 position)
        {
            yield return Click(button, (int)position.x, (int)position.y);
        }

        public static void MouseDown(int button)
        {
            MouseEvent(DownMap(button));
        }

        public static void MouseDown(int button, int x, int y)
        {
            SetPosition(x,y);
            MouseDown(button);
        }

        public static void MouseDown(int button, Vector2 position)
        {
            MouseDown(button, (int)position.x, (int)position.y);
        }

        public static void MouseUp(int button)
        {
            MouseEvent(UpMap(button));
        }

        public static void MouseUp(int button, int x, int y)
        {
            SetPosition(x,y);
            MouseUp(button);
        }

        public static void MouseUp(int button, Vector2 position)
        {
            MouseUp(button, (int)position.x, (int)position.y);
        }


        public static Vector2 GetPosition()
        {
            MousePoint currentMousePoint;
            GetCursorPos(out currentMousePoint);
            return new Vector2(currentMousePoint.x, currentMousePoint.y);
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int x,y;

        public MousePoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}

#endif

