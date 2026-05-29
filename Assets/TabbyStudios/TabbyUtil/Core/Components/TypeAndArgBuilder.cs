using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public class TypeAndArgBuilder
    {

        private static string argSep = "__";
        private static string valueSep = "_";
        private static string typeSep = "-";
    
        public static string[] GetTypesWithArgs(string s)
        {
            var a = s.Slice(s.IndexOf(typeSep) + 1).Split(typeSep);
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = a[i].RemoveLeading(typeSep).RemoveTrailing(argSep);
            }

            return a;
        }
    
        public static (string,string[]) GetArgsForType(string s)
        {
            var type = s.Split(argSep)[0];
            var args = s.Split(argSep).Skip(1).Select(a => a.Split(valueSep)[1]).ToArray();
            return (type, args);
        }
    
        public static Dictionary<string,string[]> GetArgsForTypes(string[] s)
        {
            var dict = new Dictionary<string, string[]>();

            foreach (var a in s)
            {
                var t = GetArgsForType(a);
                Assert.IsFalse(dict.ContainsKey(t.Item1), "Attempting to add duplicate component");
                dict.Add(t.Item1,t.Item2);
            }

            return dict;
        }

    
        public static Dictionary<string,string[]> GetTypesAndArgs(string s)
        {
            if (s.EndsWith(typeSep))
                return new Dictionary<string, string[]>{{s.RemoveTrailing(typeSep), Array.Empty<string>()}};
        
            var step1 = GetTypesWithArgs(s);
            return GetArgsForTypes(step1);
        }

        public static string BuildArgString(Dictionary<string, string> args)
        {
            return string.Join(argSep, args.Where(a => !a.Value.IsNullOrEmpty()).Select(kv => ArgValuePair(kv.Key, kv.Value)));
        }
        
        public static Dictionary<string, string> GetArgsForComponent(string s)
        {
            var argValuePairs = s.Split(argSep);
            return new Dictionary<string, string>(argValuePairs.Select(TupleFromStringPair));
        }

        private static string ArgValuePair(string arg, string value)
        {
            return $"{arg}{valueSep}{value}";
        }
        
        private static KeyValuePair<string, string> TupleFromStringPair(string pair)
        {
            var s = pair.Split(valueSep);
            return new KeyValuePair<string, string>(s[0], s.Length == 2 ? s[1] : "");
        }
        
    }
}