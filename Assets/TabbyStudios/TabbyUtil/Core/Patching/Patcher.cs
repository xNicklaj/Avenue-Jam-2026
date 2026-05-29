using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public static class Patcher
    {
        public static List<Harmony> harmonies = new(){ new Harmony("TabbyUtil.Patcher") };
        private static Harmony harmony = harmonies.First();
        
    
        public static void Prefix(Action original, Action patch)
        {
            Prefix(original, patch.Method);
        }
    
        public static void Prefix(MethodBase original, Action patch)
        {
            Prefix(original, patch.Method);
        }
    
        public static void Prefix(Action original, Func<bool> patch)
        {
            Prefix(original, patch.Method);
        }
    
        public static void Prefix(MethodBase original, Func<bool> patch)
        {
            Prefix(original, patch.Method);
        }
    
        public static void Prefix(Action original, MethodInfo patch)
        {
            Assert.IsFalse(original.IsLambda(), "original.IsLambda()");
            Prefix(original.Method, patch);
        }
    
        public static void Prefix(MethodBase original, MethodInfo patch)
        {
            Assert.IsTrue(patch.IsStatic, "patch.IsStatic");
            harmony.Patch(original, prefix: new HarmonyMethod(patch));
        }
        
        public static void Postfix(MethodBase original, Action patch)
        {
            Postfix(original, patch.Method);
        }
        
        public static void Postfix(MethodBase original, MethodInfo patch)
        {
            Assert.IsTrue(patch.IsStatic, "patch.IsStatic");
            harmony.Patch(original, postfix: new HarmonyMethod(patch));
        }
    
    
        public static void UnpatchAll()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        public static void SetHarmony(string id)
        {
            harmony = harmonies.FirstOrDefault(h => h.Id == id) ?? new Harmony(id);
        }
    
        public static void SetDefaultHarmony()
        {
            harmony = harmonies.First();
        }

        private class RemapInfo
        {
            public WeakReference<object> obj;
        }
    
    }
}