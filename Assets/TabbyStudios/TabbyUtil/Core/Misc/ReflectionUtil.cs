using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    public static class ReflectionUtil
    {

        public static BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        public static BindingFlags staticFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private static Type CorrectType(this object obj)
        {
            return obj as Type ?? obj.GetType();
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static FieldInfo GetFieldInfo(this object obj, string field)
        {
            var type = obj.CorrectType();
            
            while (type != null)
            {
                var f = type.GetField(field,flags);
                if (f is not null)
                    return f;
                type = type.BaseType;
            }

            throw new Exception($"{obj} doesn't have field {field}");
        }
        
        public static List<FieldInfo> GetFieldInfos(this object obj)
        {
            return obj.CorrectType().GetFields(flags).ToList();
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static object GetFieldValue(this object obj, string field)
        {
            return obj.GetFieldInfo(field).GetValue(obj);
        }
    
        public static T GetFieldValue<T>(this object obj, string field)
        {
            return (T)GetFieldValue(obj, field);
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static void SetFieldValue(this object obj, string field, object value)
        {
            obj.GetFieldInfo(field).SetValue(obj,value);
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static PropertyInfo GetPropertyInfo(this object obj, string property)
        {
            return obj.CorrectType().GetProperty(property, flags);
        }
        
        public static List<PropertyInfo> GetPropertyInfos(this object obj)
        {
            return obj.CorrectType().GetProperties(flags).ToList();
        }
        
        public static object GetPropertyValue(this object obj, string property)
        {
            return obj.GetPropertyInfo(property).GetMethod.Invoke(obj, null);
        }
        
        public static T GetPropertyValue<T>(this object obj, string property)
        {
            return (T)obj.GetPropertyValue(property);
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------
        
        public static void SetPropertyValue(this object obj, string property, object value)
        {
            obj.GetPropertyInfo(property).SetMethod.Invoke(obj, new []{value});
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------
    
        public static MethodInfo GetMethodInfo(this object obj, string name)
        {
            return obj.CorrectType().GetMethod(name, flags);
        }
        
        public static MethodInfo GetMethodInfo(this object obj, string name, params Type[] args)
        {
            return obj.CorrectType().GetMethod(name, flags, null, args, null);
        }
        
        public static List<MethodInfo> GetMethodInfos(this object obj)
        {
            return obj.CorrectType().GetMethods(flags).ToList();
        }
        
        public static object InvokeMethod(this object obj, string method, params object[] args)
        {
            return obj.GetMethodInfo(method).Invoke(obj, args);
        }
    
        public static T InvokeMethod<T>(this object obj, string method, params object[] args)
        {
            return (T)obj.GetMethodInfo(method).Invoke(obj, args);
        }
    
        public static object InvokeStaticMethod(this Type type, string name, params object[] args)
        {
            var method  = type.GetMethod(name, staticFlags, null, args.Select(obj => obj.GetType()).ToArray(), null);
            Assert.IsNotNull(method, $"Couldn't find method {name}");
            return method.Invoke(null, args);
        }
    
        public static T InvokeStaticMethod<T>(this Type type, string name, params object[] args)
        {
            return (T)InvokeStaticMethod(type, name, args);
        }
        
        public static object InvokeStaticMethodIgnoreTypes(this Type type, string name, params object[] args)
        {
            var method  = type.GetMethod(name, staticFlags);
            Assert.IsNotNull(method, $"Couldn't find method {name}");
            return method.Invoke(null, args);
        }
    
        public static T InvokeStaticMethodIgnoreTypes<T>(this Type type, string name, params object[] args)
        {
            return (T)InvokeStaticMethodIgnoreTypes(type, name, args);
        }
        
        public static object InvokeGenericMethod(this object obj, string name, Type typeParam, params object[] args)
        {
            MethodInfo method = obj.GetType().GetMethods(flags).First(m => m.Name.Equals(name) && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == args.Length);
            MethodInfo genericMethod = method.MakeGenericMethod(typeParam);
            return genericMethod.Invoke(obj, args);
        }
        
        public static object InvokeStaticGenericMethod(this Type type, string name, Type typeParam, params object[] args)
        {
            MethodInfo method = type.GetMethod(name, flags);
            MethodInfo genericMethod = method.MakeGenericMethod(typeParam);
            return genericMethod.Invoke(null, args);
        }
        
        public static object InvokeGenericMethod(this object obj, string name, Type typeParam1, Type typeParam2, params object[] args)
        {
            MethodInfo method = obj.GetType().GetMethod(name, flags);
            MethodInfo genericMethod = method.MakeGenericMethod(typeParam1, typeParam2);
            return genericMethod.Invoke(obj, args);
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------


        public static List<ConstructorInfo> GetConstructors(this object obj)
        {
            Assert.IsNotNull(obj, "Trying to get constructor on null obj");
            return obj.CorrectType().GetConstructors(flags).ToList();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------
        
        public static bool SameGenericType(this object obj, object other)
        {
            var objType = obj.GetType();
            var otherType = other.GetType();
        
            if (objType.IsGenericType ^ otherType.IsGenericType)
                return false;

            if (!objType.IsGenericType)
                return objType == otherType;
            
            return objType.GetGenericTypeDefinition() ==  otherType.GetGenericTypeDefinition();
        }
    
        public static bool SameGenericType(this Type objType, Type otherType)
        {
        
            if (objType.IsGenericType ^ otherType.IsGenericType)
                return false;

            if (!objType.IsGenericType)
                return objType == otherType;
            
            return objType.GetGenericTypeDefinition() ==  otherType.GetGenericTypeDefinition();
        }
    
        //-------------------------------------------------------------------------------------------------------------------------------------

    
        public static bool IsA(this object obj, Type type)
        {
            return type.IsAssignableFrom(obj.GetType());
        }
    
        public static bool IsA(this Type type, Type other)
        {
            return other.IsAssignableFrom(type);
        }
    
        public static bool IsA<T>(this object obj)
        {
            return typeof(T).IsAssignableFrom(obj.GetType());
        }
    
        public static bool IsA<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    
        //-------------------------------------------------------------------------------------------------------------------------------------
        
        public static MemberInfo GetMemberInfo(this object obj, string member)
        {
            //Assert.AreEqual(1, obj.CorrectType().GetMember(member, flags).Length, "Not ready");
            return obj.CorrectType().GetMembers(flags).AssertFirst(m => m.Name == member);
        }
        
        public static List<MemberInfo> GetMemberInfos(this object obj)
        {
            return obj.GetType().GetMembers(flags).Where(m => m is FieldInfo or PropertyInfo).ToList();
        }
        
        public static object GetMemberValue(this object obj, string member)
        {
            //seems to be around 5 times slower than GetFieldValue
            var m = obj.GetMemberInfo(member);
            return m is FieldInfo ? obj.GetFieldValue(member) : obj.GetPropertyValue(member);
        }
        
        public static T GetMemberValue<T>(this object obj, string member)
        {
            var m = obj.GetMemberInfo(member);
            return (T)(m is FieldInfo ? obj.GetFieldValue(member) : obj.GetPropertyValue(member));
        }
        
        public static List<object> GetMemberValues(this object obj)
        {
            var members = obj.GetMemberInfos();
            return members.Select(m => m is FieldInfo ? obj.GetFieldValue(m.Name) : obj.GetPropertyValue(m.Name)).ToList();
        }
        
        public static Dictionary<string, object> GetMemberMap(this object obj)
        {
            var members = obj.GetMemberInfos();
            return new Dictionary<string, object>(members.Select(m => new KeyValuePair<string, object>(m.Name, obj.GetMemberValue(m.Name))));
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------
        
        public static Type FindType(string name, bool fullName = true)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
            return types.FirstOrDefault(t => name == (fullName ? t.FullName : t.Name));
        }
        
        public static void DiscoverTypes(string name)
        { 
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.FullName.Contains(name));
            types.ToList().ForEach(t => Debug.Log(t.FullName));
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------
        
        public static PropertyInfo GetInterfaceProperty(this Type concreteType, string propertyName)
        {
            return concreteType.GetInterfaceMap(concreteType.GetInterfaces().First(i => i.GetProperty(propertyName) != null)).InterfaceType.GetProperty(propertyName);
        }
        
        
        public static Type GetInterfaceType(this object obj, Type genericInterface)
        {
            return obj.GetType().GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
        }

        public static Type GetGenericArgument(this object obj, Type genericInterface)
        {
            var interfaceType = GetInterfaceType(obj, genericInterface);
            return interfaceType?.GetGenericArguments().FirstOrDefault();
        }
        
        public static void AssignFieldFromString(this object obj, string fieldName, string stringValue)
        {
            var field = obj.GetFieldInfo(fieldName);

            try
            {
                Type fieldType = field.FieldType;
                object convertedValue = null;
            
                if (fieldType == typeof(int))
                {
                    convertedValue = int.Parse(stringValue);
                }
                else if (fieldType == typeof(float))
                {
                    convertedValue = float.Parse(stringValue);
                }
                
                field.SetValue(obj, convertedValue);
            }
            catch (Exception)
            {
                Debug.Log("Bad Value");
            }
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------
        // idk what's going on with these
        
        // public static ConstructorInfo GetConstructorInfo(this object obj)
        // {
        //     var constructors = obj.CorrectType().GetConstructors(flags).Where(c => !c.IsStatic).ToList();
        //     if (constructors.IsEmpty())
        //         return null;
        //     
        //     Assert.IsTrue(constructors.One());
        //     return constructors.First();
        // }
        //
        // public static List<ConstructorInfo> GetConstructorInfos(this object obj)
        // {
        //     return obj.GetType().GetConstructors(flags).ToList();
        // }
        
        //-------------------------------------------------------------------------------------------------------------------------------------

        
    }
}