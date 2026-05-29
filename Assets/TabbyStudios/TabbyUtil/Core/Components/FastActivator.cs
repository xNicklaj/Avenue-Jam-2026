using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace TabbyStudios
{
    public static class FastActivator
    {
        private static ConcurrentDictionary<Type, Func<object[], object>> Cache = new();
        private static bool fallback;
    
        static FastActivator()
        {
            fallback = !RuntimeFeature.IsDynamicCodeSupported;
        }
    
        public static object CreateInstance(Type type, object[] args = null)
        {
            if (fallback) return Activator.CreateInstance(type, args);
            var factory = Cache.GetOrAdd(type, CreateFactory);
            return factory(args);
        }
    
        private static Func<object[], object> CreateFactory(Type type)
        {
            var ctor = type.GetConstructors()[0];
            var parameters = ctor.GetParameters();
    
            var dm = new DynamicMethod(
                "Create_" + type.Name,
                typeof(object),
                new[] { typeof(object[]) },
                typeof(FastActivator).Module,
                skipVisibility: true);
    
            var il = dm.GetILGenerator();
    
            for (int i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_0); // load args array
                il.Emit(OpCodes.Ldc_I4, i); // load index
                il.Emit(OpCodes.Ldelem_Ref); // get args[i]
    
                if (parameters[i].ParameterType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
                else
                    il.Emit(OpCodes.Castclass, parameters[i].ParameterType);
            }
    
            il.Emit(OpCodes.Newobj, ctor);
    
            if (type.IsValueType)
                il.Emit(OpCodes.Box, type);
    
            il.Emit(OpCodes.Ret);
    
            return (Func<object[], object>)dm.CreateDelegate(typeof(Func<object[], object>));
        }
    }
}
