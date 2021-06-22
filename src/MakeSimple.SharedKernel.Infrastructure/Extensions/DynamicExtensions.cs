using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    /// <summary>
    /// Source: https://github.com/cloudnative-netcore/netcorekit/blob/d07dc233cc82f78a44c03b346eaddf460a33da2c/src/NetCoreKit.Utils/Extensions/DynamicExtensions.cs
    /// </summary>
    public static class DynamicExtensions
    {
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            return expando as ExpandoObject;
        }
    }
}