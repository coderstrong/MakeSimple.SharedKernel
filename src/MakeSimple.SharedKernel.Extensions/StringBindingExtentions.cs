using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MakeSimple.SharedKernel.Extensions
{
    /// <summary>
    /// Refer: https://gist.githubusercontent.com/enisn/f8f58d8bbc98ef5853f19e1a8d7cc1f9/raw/b1c2c1d417887e53f5604313eaeb5f6e7e0d9bb7/BindingExtensions.cs
    /// </summary>
    public static class StringBindingExtentions
    {
        /// <summary>
        /// Binding properties name into string format
        /// </summary>
        /// <param name="str">string format ex: "for example: {Name}, For all {Class.Name}"</param>
        /// <param name="obj">
        /// {
        ///     Name: "Joh",
        ///     Class: {
        ///         Name: "CK01"
        ///     }
        /// }
        /// </param>
        /// <returns></returns>
        public static string BindObjectProperties(this string str, object obj)
        {
            if (obj == null) return str;
            foreach (var item in ExtractParams(str))
            {
                str = str.Replace("{" + item + "}", obj.GetPropValue(item)?.ToString());
            }
            return str;
        }

        public static object GetPropValue(this object obj, string name)
        {
            foreach (string part in name.Split('.'))
            {
                if (obj == null) { return null; }
                if (obj.IsNonStringEnumerable())
                {
                    var toEnumerable = (IEnumerable)obj;
                    var iterator = toEnumerable.GetEnumerator();
                    if (!iterator.MoveNext())
                    {
                        return null;
                    }
                    obj = iterator.Current;
                }
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        private static IEnumerable<string> ExtractParams(string str)
        {
            var splitted = str.Split('{', '}', StringSplitOptions.None);
            for (int i = 1; i < splitted.Length; i += 2)
                yield return splitted[i];
        }

        private static bool IsNonStringEnumerable(this object instance)
        {
            return instance != null && instance.GetType().IsNonStringEnumerable();
        }

        private static bool IsNonStringEnumerable(this Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}