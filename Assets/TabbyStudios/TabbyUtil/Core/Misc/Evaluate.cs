using System;
using System.Linq.Expressions;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public class Evaluate
    {
        public static void Fail()
        {
            throw new AssertionException("Assert.Fail()", null);
        }
        
        public static void Fail(string msg)
        {
            throw new AssertionException(msg, null);
        }
        
        public static void True(Expression<Func<bool>> condition)
        {
            Assert.IsTrue(condition.Compile().Invoke(), $"Expression was false: {LogUtil.ConditionString(condition)}");
        }
    }
}