using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

//using NUnit.Framework;

namespace TabbyStudios
{
    public static class StringExtensions
    {
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static string AddTrailing(this string s, string end)
        {
            if (!s.AsSpan().EndsWith(end))
                return s + end;
            return s;
        }
        
        public static string AddLeading(this string s, string start)
        {
            if (!s.AsSpan().StartsWith(start))
                return start + s;
            return s;
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static string RemoveTrailing(this string s, string end)
        {
            if (s.AsSpan().EndsWith(end))
            {
                return s.Substring(0, s.Length - end.Length);
            }
            return s;
        }
        
        public static string RemoveTrailing(this string s, params string[] end)
        {
            if (end.Many(s.Contains))
            {
                throw new Exception("Result depends on order of params");
            }

            foreach (var e in end)
            {
                s = s.RemoveTrailing(e);
            }
            
            return s;
        }

        public static string RemoveLeading(this string s, string start)
        {
            if (s.AsSpan().StartsWith(start))
            {
                return s.Substring(start.Length);
            }
            return s;
        }
        
        public static string RemoveLeading(this string s, params string[] start)
        {
            if (start.Many(s.Contains))
            {
                throw new Exception("Result depends on order of params");
            }

            foreach (var e in start)
            {
                s = s.RemoveLeading(e);
            }
            
            return s;
        }
    
        public static string RemoveFirst(this string s, string toRemove)
        {
            int index = s.IndexOf(toRemove);
            if (index < 0) return s;
            return s.Remove(index, toRemove.Length);
        }

        public static string RemoveMultiple(this string s, string toRemove, int count = -1)
        {
            string result = s;
            int occurrences = 0;
            while (result.Contains(toRemove) && (count == -1 || occurrences < count))
            {
                result = result.Remove(result.IndexOf(toRemove), toRemove.Length);
                occurrences++;
            }
            return result;
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------


        public static string ReplaceFirst(this string s, string match, string replacement)
        {
            int index = s.IndexOf(match);
            if (index < 0) return s;
            return s.Substring(0, index) + replacement + s.Substring(index + match.Length);
        }

        public static string ReplaceMultiple(this string s, string match, string replacement, int count = -1)
        {
            string result = s;
            int occurrences = 0;
            while (result.Contains(match) && (count == -1 || occurrences < count))
            {
                int index = result.IndexOf(match);
                result = result.Substring(0, index) + replacement + result.Substring(index + match.Length);
                occurrences++;
            }
            return result;
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static int IndexOfNthOccurrence(this string s, string substring, int n) {
            int index = -1;
            int startIndex = 0;

            for (int i = 0; i < n; i++) {
                index = s.IndexOf(substring, startIndex);
                if (index == -1) 
                    return -1;
                startIndex = index + 1;
            }

            return index;
        }
        
        public static int IndexOfNthOccurrence(this string source, char value, int n)
        {
            if (source == null || n <= 0) return -1;
    
            var span = source.AsSpan();
            int count = 0;
    
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == value)
                {
                    if (++count == n)
                        return i;
                }
            }
    
            return -1;
        }
        
        public static string RemoveAfterOccurence(this string s, string separator, int occurence, bool removeSeparator=true)
        {
            int index = s.IndexOfNthOccurrence(separator,occurence);
            if (index != -1)
            {
                return s.Substring(0, index + (removeSeparator ? 0 : separator.Length));
            }
            
            return s;
        }
        
        public static string RemoveAfterOccurence(this string s, char separator, int occurence)
        {
            int index = s.IndexOfNthOccurrence(separator,occurence);
            if (index != -1)
            {
                return s.Substring(0, index);
            }
            
            return s;
        }
        
        public static string RemoveBeforeOccurence(this string s, string separator, int occurence, bool removeSeparator=true)
        {
            int index = s.IndexOfNthOccurrence(separator,occurence);
            if (index != -1)
            {
                return s.Substring(index, index + (removeSeparator ? 0 : separator.Length));
            }
            
            return s;
        }
        
        public static string RemoveAfterFirst(this string s, string separator, bool removeSeparator = true)
        {
            var index = s.IndexOf(separator);
            return index != -1 ? s.Substring(0, index) : s;        
        }
        
        public static string RemoveAfterLast(this string s, string separator, bool removeSeparator=true)
        {
            var index = s.AsSpan().LastIndexOf(separator);
            return index != -1 ? s.Substring(0, index) : s;  
        }
    
        public static string RemoveBeforeLast(this string s, string separator, bool removeSeparator=true)
        {
            int lastIndex = s.AsSpan().LastIndexOf(separator);
            if (lastIndex != -1)
            {
                if(removeSeparator)
                    return s.Substring(lastIndex+separator.Length, s.Length-lastIndex-1);
                return s.Substring(lastIndex, s.Length-lastIndex-1);
            }
            return s;
        }
        
        public static string RemoveBeforeFirst(this string s, string separator)
        {
            int index = s.IndexOf(separator);
            return index != -1 ? s.Substring(index + separator.Length) : s;
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static List<string> Step(this string s, string separator, bool includeLast = false)
        {
            var result = new List<string>();
            var current = "";

            //Assert.AreEqual(1, separator.Length, "Step can't handle this case yet");
            
            if (includeLast)
                s = s.AddTrailing(separator);
            
            foreach (var c in s)
            {
                if (c.ToString() == separator)
                    result.Add(current);
                current += c;
            }
            
            return result;
        }
        
        public static int Count(this string s, string t)
        {
            return t.Split(s).Length - 1;
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return s is null or "";
        }

        public static string IfNullOrEmpty(this string s, string fallback)
        {
            return s.IsNullOrEmpty() ? fallback : s;
        }
        
        public static string RemoveDigits(this string s)
        {
            return new string(s.Where(c => !char.IsDigit(c)).ToArray());
        }

        public static string FixSlashes(this string s)
        {
            return s.ReplaceMultiple("\\", "/");
        }
    
        public static string Uncapitalize(this string s) => string.IsNullOrEmpty(s) ? s : char.ToLower(s[0]) + s.Substring(1);
        public static string Capitalize(this string s) => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);
        
        public static string ReadableVarName(this string input) => Regex.Replace(input, "(?<!^)([A-Z])", " $1").Capitalize();
        public static string VarNameFromReadableString(this string input) => input.RemoveMultiple(" ").Uncapitalize();
        
        public static string ToPascalCase(this string input) => string.Concat(input.RemoveLeading("-").Split('-').Select(word => char.ToUpper(word[0]) + word.Substring(1)));
        

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}