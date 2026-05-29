using System;
using System.Collections;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public class RunAsync
    {
        public static void Fire(int intervalMilliseconds, Action action)
        {
            var synchronizationContext = SynchronizationContext.Current;
            Task.Run(async () =>
            {
                await Task.Delay(intervalMilliseconds);
                synchronizationContext.Post(_ => action(), null);
            });
        }
        
        public static Task FireTask(int intervalMilliseconds, Action action)
        {
            var synchronizationContext = SynchronizationContext.Current;
            var task = Task.Run(async () =>
            {
                await Task.Delay(intervalMilliseconds);
                synchronizationContext.Post(_ => action(), null);
            });
            return task;
        }
        
        public static IEnumerator WaitUntil(Expression<Func<bool>> condition, float msTimeout = 1000)
        {
            var func = condition.Compile();
            if(func()) LogUtil.Log($"Warning: condition {condition} was already true");
            #if UNITY_6000_0_OR_NEWER
            yield return new WaitUntil(func, TimeSpan.FromMilliseconds(msTimeout), () => Evaluate.Fail($"Timed out waiting for {LogUtil.ConditionString(condition)}"));
            #else
            yield return new WaitUntil(func);
            #endif
            
            Assert.IsTrue(func());
        }
    }
}