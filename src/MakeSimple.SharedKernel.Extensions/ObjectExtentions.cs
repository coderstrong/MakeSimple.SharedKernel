using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MakeSimple.SharedKernel.Extensions
{
    public static class ObjectExtentions
    {
        public enum NameConvention
        {
            Default,
            SnakeCase,
        }

        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source, NameConvention convention = NameConvention.Default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "Unable to convert object to a dictionary. The source object is null.");

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary, convention);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary, NameConvention convention)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
            {
                switch (convention)
                {
                    case NameConvention.SnakeCase:
                        dictionary.Add(property.Name.ToSnakeCase(), (T)value);
                        break;

                    default:
                        dictionary.Add(property.Name, (T)value);
                        break;
                }
            }
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }
    }
}