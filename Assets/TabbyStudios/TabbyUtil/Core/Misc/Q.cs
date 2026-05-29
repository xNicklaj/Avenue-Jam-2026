using System;
using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public static class Q
    {
        public static T[] A<T>(params T[] args) => args;
        public static List<T> L<T>(params T[] args) => args.ToList();

        public static IEnumerable<int> R(int end) => Enumerable.Range(0, end);
        public static IEnumerable<int> R(int start, int end) => Enumerable.Range(start, end);

        public static List<T> L<T>(Func<int, T> func, int count) => R(count).Select(func).ToList();
        public static List<T> L<T>(Func<int, T> func, int start, int end) => R(start, end).Select(func).ToList();
    }
}