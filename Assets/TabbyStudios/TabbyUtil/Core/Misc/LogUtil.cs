using System;
using System.Linq.Expressions;
using UnityEngine;

namespace TabbyStudios
{
    public class LogUtil
    {
        public static void Log(params object[] msgs)
        {
            Debug.Log(string.Join(", ", msgs));
        }

        public static void Log(string msg, Color color)
        {
            Debug.Log($"<color=#{ToHex(color)}>{msg}</color>");
        }

        private static string ToHex(Color color)
        {
            string AsHex(float val) => ((int)(255*val)).ToString("X");
            return $"{AsHex(color.r)}{AsHex(color.g)}{AsHex(color.b)}";
        }

        public static string ConditionString<T>(Expression<Func<T>> condition)
        {
            return condition.ToString();
        }
        
        public static string ConditionString<T, U>(Expression<Func<T, U>> condition)
        {
            return condition.ToString();
        }
    }
}