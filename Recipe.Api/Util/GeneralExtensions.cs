using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Recipe.Api.Models.Abstract;
using Recipe.Api.Models.DefaultContextModels.GroupsAndTypesModels;

namespace Recipe.Api.Util
{
    public static class GeneralExtensions
    {
        public static string ToHex(this int input)
        {
            return input.ToString("X");
        }
        
        public static string ToHex(this ushort input)
        {
            return input.ToString("X");
        }
        
        public static string ToNCharHex(this int input, int n)
        {
            var val =  input.ToString("X");
            if(val.Length > n)
                throw new ArgumentException($"Input should only produce {n} character hex, resulting value was: " + val);
            while (val.Length < n)
            {
                val = "0" + val;
            }
            return val;
        }
        
        public static int ParseHexAsInt(this string input)
        {
            return int.Parse(input, NumberStyles.HexNumber);
        }

        public static string ParseIpAddressFromHex(this string input)
        {
            if(input.Length < 8)
                throw new ArgumentException("Cannot parse Ip Address from a hex string less than 8 characters, input: " + input);
            string ipAddress = "";
            for (var i = 0; i < 8; i+=2)
            {
                ipAddress += input.Substring(i, 2).ParseHexAsInt();
                if (i != 6)
                    ipAddress += ".";
            }

            return ipAddress;
        }

        public static DateTime ConvertToDateTimeFromUnixTimeStamp(this long unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
        
        public static string ConvertPascalToTitleCase(this string str)
        {
            return Regex.Replace(str, "([A-Z])", " $1").Trim();
        }
        
        public static string ConvertToVariableName(this string str)
        {
            var replaced = 
                Regex.Replace(
                Regex.Replace(
                Regex.Replace(str, @"[^a-zA-Z0-9]|\s+|-", "_").Trim('_'),
                "_+",
                "_"
            ), "-+", "_");
            if (Regex.IsMatch(replaced, @"^\d"))
                return "_" + replaced;
            return replaced;
        }
        
        public static bool GetBit(this uint intVal, byte bitNumber)
        {
            // Uses Bitwise AND with a number that has a bit shifted to the left by the bitNumber
            return (intVal & (1 << bitNumber)) != 0;
        }
        
        public static bool GetBit(this uint intVal, int bitNumber)
        {
            if (bitNumber < 0)
            {
                throw new ArgumentException("Must be greater or equal to 0", nameof(bitNumber));
            }
            // Uses Bitwise AND with a number that has a bit shifted to the left by the bitNumber
            return (intVal & (1 << bitNumber)) != 0;
        }
        
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static bool IsNullOr0(this int? value)
        {
            return !value.HasValue || value == 0;
        }
        
        public static StringBuilder AppendTabbedLine(this StringBuilder sBuilder, string str, int tabs = 1)
        {
            string tabsStr = "";
            for (int i = 0; i < tabs; i++)
                tabsStr += "\t";
            return sBuilder.AppendLine(tabsStr + str);
        }

        public static void ValidateListOrder(this IEnumerable<IOrderable> list)
        {
            var ascendingList = list.OrderBy(a => a.Order).ToList();
            for (int i = 0; i < ascendingList.Count; i++)
                ascendingList[i].Order = i;
        }

        /**
         * Converts the IEnumerable of tasks to a task with the results. Each task is executed sequentially after each other.
         * This prevents the ef core error: A second operation started on this context before a previous operation completed
         * since the DBContext does not support parallel database access with the same DBContext.
         */
        public static async Task<IEnumerable<T>> ToSequentialTask<T>(this IEnumerable<Task<T>> list)
        {
            var newList = new List<T>();
            foreach (var task in list)
            {
                newList.Add(await task);
            }
            return newList;
        }
        
        public static bool IsOneOf(this Enum enumeration, params Enum[] enums)
        {
            return enums.Contains(enumeration);
        }
        
        public static IEnumerable<T> FilterAndCast<T>(this IEnumerable<object> source) where T : class
        {
            return source.Where(child => child is T).Cast<T>();
        }

        public static T ThrowIfNull<T>(this T source, string paramName, string message = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(paramName, message);
            }
            return source;
        }
        
        public static void CheckNulls(object[] paramsToCheck, 
            [CallerMemberName] string callerName = null, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (paramsToCheck.Length % 2 == 1)
            {
                throw new ArgumentException("wrong number of arguments");
            }
            var keyValuePairs = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < paramsToCheck.Length; i += 2)
            {
                keyValuePairs.Add(new KeyValuePair<string, object>((string)paramsToCheck[i], paramsToCheck[i + 1]));
            }
            foreach (var keyValuePair in keyValuePairs)
            {
                keyValuePair.Value.ThrowIfNull(
                    keyValuePair.Key,
                    callerName != null ? $"Method: {callerName} File: {sourceFilePath} Line: {sourceLineNumber} " : null
                );
            }
        }
        
        public static void CheckNulls([NotNull] this object ob, object[] paramsToCheck, 
            [CallerMemberName] string callerName = null,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            CheckNulls(paramsToCheck, callerName, ob.GetType().Name, sourceLineNumber);
        }
        
        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // do not modify "guard" values
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
        
        public static async Task DoAsync<T>(this T source, Func<T, Task> action)
        {
            await action(source);
        }
        
        public static async Task TupleDoAsync<T, R>(this Tuple<T, R> source, Func<T, R, Task> action)
        {
            await action(source.Item1, source.Item2);
        }
        public static async Task TupleDoAsync<T>(this ValueTuple<T, T> source, Func<T, T, Task> action)
        {
            await action(source.Item1, source.Item2);
        }

        public static TResult Expand<T, TResult>(this T source, Func<T, TResult> action)
        {
            return action(source);
        }
        public static TResult Pipe<T, TResult>(this T source, Func<T, TResult> action)
        {
            return action(source);
        }
        
        public static List<int> ToIds<TSource>(this IEnumerable<TSource> source) where TSource : IPrimaryKeyModel
        {
            return source.Select(r => r.Id).ToList();
        }
        
    }
}