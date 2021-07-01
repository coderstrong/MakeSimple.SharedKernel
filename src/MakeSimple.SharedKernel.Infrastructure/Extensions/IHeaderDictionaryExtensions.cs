using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    public static class IHeaderDictionaryExtensions
    {
        public static string CovertToString(this IHeaderDictionary headers, bool isIgnoreToken = true)
        {
            IEnumerable<KeyValuePair<string, StringValues>> query = headers.AsEnumerable();

            if (isIgnoreToken)
            {
                query = headers.Where(kvp => kvp.Key != "Authorization");
            }

            return string.Join(", ", query.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToArray());
        }
    }
}