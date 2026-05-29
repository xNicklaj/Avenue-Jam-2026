using System;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    [InitializeOnLoad]
    public static class EditorUtil
    {
        public static float defaultTimeout = 1;
        public static float windowMinDistanceToScreenEdge => 8;
        public static float arbitrarySendToSecondScreenLowerBound => 1/3f;
        
        public static Vector2 screenSize => new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        public static bool doingInitializeOnLoad { get; private set; }
        
        static EditorUtil()
        {
            doingInitializeOnLoad = true;
            EditorApplication.delayCall += () => doingInitializeOnLoad = false;
            RunAsync.Fire(500, () => doingInitializeOnLoad = false);
        }
        
        public static void DelayCall(Action call)
        {
            DelayCall(1, call);
        }

        public static void DelayCall(int delay, Action call)
        {
            if (delay - 1 < 0)
            {
                call();
            }
            else
            {
                EditorApplication.delayCall += () => DelayCall(delay - 1, call);
            }
        }

        public static void DoAfterLoad(Action call)
        {
            DelayCall(2, call);
        }
        
        public static void UpdateDelayCall(int delay, Action call)
        {
            EditorApplication.update += new UpdateWrapper(delay, call).Start;
        }
        
        public static void UpdateDelayCall(Action call)
        {
            UpdateDelayCall(1, call);
        }
        
        public static void UpdateWaitForCondition(Action call, Func<bool> condition, float timeout = 1f)
        {
            EditorApplication.update += new UpdateConditionWrapper(call, condition, timeout).Start;
        }
        
        public static void CallEveryXUpdates(int x, Action call)
        {
            EditorApplication.update += new UpdateWrapper(x, call, repeating:true).Start;
        }
        
        private class UpdateWrapper
        {
            private Action call;
            private int cooldown, delay;
            private bool repeating;

            public UpdateWrapper(int delay, Action call, bool repeating = false)
            {
                this.call = call;
                this.cooldown = delay;
                this.delay = delay;
                this.repeating = repeating;
            }

            public void Start()
            {
                cooldown--;
                if (cooldown > 0)
                    return;
                if (repeating)
                    cooldown = delay;
                else
                    EditorApplication.update -= Start;
                call();
            }
        }

        private class UpdateConditionWrapper
        {
            private Action call;
            private Func<bool> condition;
            private float timeout;
            private double time, last;

            public UpdateConditionWrapper(Action call, Func<bool> condition, float timeout = 1)
            {
                this.call = call;
                this.condition = condition;
                this.timeout = timeout;
                last = Time.realtimeSinceStartupAsDouble;
            }

            public void Start()
            {
                try
                {
                    time += Time.realtimeSinceStartupAsDouble - last;
                    last = Time.realtimeSinceStartupAsDouble;

                    if (condition())
                    {
                        EditorApplication.update -= Start;
                        call();
                    }

                    if (time > timeout && timeout > 0)
                    {
                        EditorApplication.update -= Start;
                        throw new Exception("Timeout in UpdateWaitForCondition");
                    }
                }
                catch
                {
                    EditorApplication.update -= Start;
                    throw;
                }
            }
        }

        public static string CurrentLayoutName()
        {
            return ReflectionUtil.FindType("UnityEditor.Toolbar").GetPropertyValue<string>("lastLoadedLayoutName");
        }

        public static Color ReadPixelLowPrecision(float x, float y)
        {
            return ReadPixelLowPrecision(new(x,y));
        }
        
        public static Color ReadPixelLowPrecision(Vector2 pos)
        {
            return UnityEditorInternal.InternalEditorUtility.ReadScreenPixel(pos, 1, 1)[0];
        }
        
    }
}