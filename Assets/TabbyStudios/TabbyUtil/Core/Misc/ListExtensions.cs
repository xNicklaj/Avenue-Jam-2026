using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

namespace TabbyStudios
{
    public static class ListExtensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static T Pop<T>(this List<T> list)
        {
            T item = list[^1];
            list.RemoveAt(list.Count - 1);
            return item;
        }
    
        public static T Dequeue<T>(this List<T> list)
        {
            T item = list[0];
            list.RemoveAt(0);
            return item;
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static List<T> ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            var list = e.ToList();
            list.ForEach(action);
            return list;
        }
    
        public static List<T> ForEach<T>(this IEnumerable<T> e, Action<T,int> action)
        {
            var list = e.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                action(list[i], i);
            }
            return list;
        }
        
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        private static (int, int) SliceBounds<T>(IEnumerable<T> e, int start, int end, int count = -1)
        {
            if(count == -1)
                count = e.Count();
            
            if(start < 0)
                start = count + start;
        
            if (end == int.MaxValue)
                end = count;
            else if(end < 0)
                end = count + end;

            return (start, end);
        }
        
        public static List<T> Slice<T>(this IEnumerable<T> e, int start, int end = int.MaxValue)
        {
            var list = e.ToList();
            (start,end) = SliceBounds(list, start, end);
            return list.Skip(start).Take(end - start).ToList();
        }
        
        public static List<T> Slice<T>(this List<T> e, int start, int end = int.MaxValue)
        {
            if (start == 0 && end == int.MaxValue)
                return e;

            return Slice((IEnumerable<T>)e, start, end);
        }
        
        public static string Slice(this string e, int start, int end = int.MaxValue)
        {
            (start,end) = SliceBounds(e, start, end, count:e.Length);
            return e.Substring(start, end-start);
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------
    
        public static bool HasDuplicates<T>(this IEnumerable<T> e)
        {
            IEnumerable<T> enumerable = e.ToList();
            return enumerable.ToList().Distinct().Count() != enumerable.ToList().Count();
        }
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e is null || !e.Any();
        }
        
        public static bool IsEmpty<T>(this IEnumerable<T> e)
        {
            return !e.Any();
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static bool One<T>(this IEnumerable<T> e)
        {
            return e.Count() == 1;
        }
        
        public static bool One<T>(this IEnumerable<T> e, Func<T,bool> pred)
        {
            return e.Count(pred) == 1;
        }
        
        public static bool Many<T>(this IEnumerable<T> e)
        {
            return e.Count() > 1;
        }
        
        public static bool Many<T>(this IEnumerable<T> e, Func<T,bool> pred)
        {
            return e.Count(pred) > 1;
        }
        
        public static bool None<T>(this IEnumerable<T> e, Func<T,bool> prop)
        {
            return e.All(t => !prop(t));
        }
        
        public static bool None<T>(this IEnumerable<T> e)
        {
            return e.IsEmpty();
        }
        
        public static T WithMax<T>(this IEnumerable<T> e, Func<T,float> prop)
        {
            return e.Aggregate((min, next) => prop(next) > prop(min) ? next : min);
        }
        
        public static T WithMin<T>(this IEnumerable<T> e, Func<T,float> prop) where T : class
        {
            return e.Aggregate((min, next) => prop(next) < prop(min) ? next : min);
        }
        
        public static T Unique<T>(this IEnumerable<T> e, Func<T,bool> pred)
        {
            var matches = e.Where(pred).ToList();
            Assert.IsTrue(matches.One());
            return matches.First();
        }
        
        public static T Before<T>(this IEnumerable<T> e, T item)
        {
            var list = e.ToList();
            return list[list.IndexOf(item) - 1];
        }
        
        public static List<T> ItemsBefore<T>(this IEnumerable<T> e, T item)
        {
            var list = e.ToList();
            return list.Slice(0, list.IndexOf(item));
        }
        
        public static T After<T>(this IEnumerable<T> e, T item)
        {
            var list = e.ToList();
            return list[list.IndexOf(item) + 1];
        }
        
        public static List<T> ItemsAfter<T>(this IEnumerable<T> e, T item)
        {
            var list = e.ToList();
            var index = list.IndexOf(item);
            if (index > list.Count) return new();
            return list.Slice(index + 1);
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        
        public static bool IsSorted<T>(this IEnumerable<T> e, Func<T,T,bool> pred)
        {
            var list = e.ToList();
            return list.Zip(list.Skip(1), pred).All(x => x);
        }
        
        public static bool IsSorted<T>(this IEnumerable<T> e, Func<T,float> prop)
        {
            var list = e.ToList();
            return list.Zip(list.Skip(1), (f,s) => prop(s) >= prop(f)).All(x => x);
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static T Random<T>(this IEnumerable<T> e)
        {
            var list = e.ToList();
            return list.ElementAt(new Random().Next(0, list.Count));
        }
        
        public static T Random<T>(this IEnumerable<T> e, Func<T,bool> pred)
        {
            var list = e.ToList();
            return list.Where(pred).ElementAt(new Random().Next(0, list.Count));
        }
        
        public static T Random<T>(this IEnumerable<T> e, Func<T,int,bool> pred)
        {
            var list = e.ToList();
            var filtered = list.Where(pred).ToList();
            var index = new Random().Next(0, filtered.Count);
            return filtered.ElementAt(index);
        }
        
        public static List<T> Shuffled<T>(this IList<T> list)
        {
            List<T> copy = new List<T>(list);
            int n = copy.Count;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0, n--);
                (copy[n], copy[k]) = (copy[k], copy[n]);
            }
            return copy;
        }
        
        public static List<T> DeterministicShuffled<T>(this IList<T> list, int seed)
        {
            Random rng = new Random(seed);
            List<T> copy = new List<T>(list);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (copy[n], copy[k]) = (copy[k], copy[n]);
            }

            return copy;
        }
        
        public static T AssertFirst<T>(this IEnumerable<T> e, Expression<Func<T, bool>> pred)
        {
            var items = e.Where(pred.Compile()).ToList();
            Assert.AreNotEqual(0, items.Count, $"Enumerable contains no elements such that {LogUtil.ConditionString(pred)}");
            return items.First();
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        
        public static List<(T t, U u)> SelectPredTuple<T, U>(this IEnumerable<T> list, Func<T, U> getter, Func<U, bool> pred) where U : class
        {
            return list.Select(t => (t, u:getter(t))).Where(tu => pred(tu.u)).ToList();
        }
        
        public static List<(T t, U u)> SelectNonNullTuple<T, U>(this IEnumerable<T> list, Func<T, U> getter) where U : class
        {
            return list.SelectPredTuple(getter, u => u is not null);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        public static void Remove<T>(this List<T> list, Func<T,bool> pred)
        {
            list.RemoveAll(t => pred(t));
        }
        
        public static void RemoveRange<T>(this List<T> list, IEnumerable<T> toRemove)
        {
            var set = new HashSet<T>(toRemove);
            list.RemoveAll(set.Contains);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> e, Func<T, bool> pred, T _default)
        {
            var result = e.FirstOrDefault(pred);
            return EqualityComparer<T>.Default.Equals(result, default) ? _default : result;
        }
        
        public static IEnumerable<T> Except<T>(this IEnumerable<T> e, T t)
        {
            return e.Except(new[] { t });
        }
        
        public static int IndexOf<T>(this List<T> e, Func<T,bool> pred)
        {
            return e.IndexOf(e.First(pred));
        }

        public static void Log<T>(this IEnumerable<T> e)
        {
            foreach (var item in e)
            {
                Debug.Log(item);
            }
        }
    }
}