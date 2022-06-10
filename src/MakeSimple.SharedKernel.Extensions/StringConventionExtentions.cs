using System.Collections.Generic;
using System.Linq;

namespace MakeSimple.SharedKernel.Extensions
{
    public static class StringConventionExtentions
    {
        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public static IEnumerable<string> BreakStringStartWith(this string str)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 1; i <= str.Length; i++)
                {
                    result.Add(str[..i]);
                }
            }
            return result;
        }
    }
}