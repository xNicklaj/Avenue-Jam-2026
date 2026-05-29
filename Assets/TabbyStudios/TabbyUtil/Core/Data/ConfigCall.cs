using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public class ConfigCall
    {
        private List<(MethodInfo method, WeakReference<object> obj)> calls = new();
        private List<MethodInfo> staticCalls = new();

        public void Invoke(object value)
        {
            RemoveGarbage();
            calls.ForEach(c => c.method.Invoke(c.obj.Target(), new [] {value}));
            staticCalls.ForEach(m => m.Invoke(null, new [] {value}));
        }

        public void Add<T>(Action<T> call)
        {
            Assert.IsFalse(call.IsLambda());
            if (call.Method.IsStatic)
            {
                staticCalls.Add(call.Method);
            }
            else
            {
                calls.Add(new(call.Method, new(call.Target)));
            }
        }
    
        public void Remove<T>(Action<T> call)
        {
            calls.Remove(t => t.method == call.Method);
            staticCalls.Remove(m => m == call.Method);
        }

        public void RemoveGarbage()
        {
            calls.Remove(t => t.obj.Target() is null);
        }

        public int Count()
        {
            RemoveGarbage();
            return calls.Count + staticCalls.Count;
        }
    
    }
}