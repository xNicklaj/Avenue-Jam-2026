using System;
using System.Collections.Generic;

namespace TabbyStudios
{
    public static class Windup
    {
        private static Dictionary<Action, WindupAction> dict = new();

        public static void Invoke(int delay, Action action)
        {
            //Assert.IsFalse(action.IsLambda());
        
            if (!dict.ContainsKey(action))
                dict[action] = new WindupAction(delay, action);
        
            dict[action].Invoke();
        }
    
        public static void Reset(Action action)
        {
            if(dict.ContainsKey(action))
                dict[action].Reset();
        }
    
        public class WindupAction
        {
            private int delay, current;
            private Action action;

            public WindupAction(int delay, Action action)
            {
                this.delay = delay;
                this.action = action;
            }

            public void Invoke()
            {
                current++;
                if (current <= delay)
                    return;
                action();
            }
        
            public void Reset()
            {
                current = 0;
            }
        }
    }
}