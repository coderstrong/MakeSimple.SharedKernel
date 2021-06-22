namespace MakeSimple.SharedKernel.Infrastructure.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Reference: https://github.com/cloudnative-netcore/netcorekit/blob/d07dc233cc82f78a44c03b346eaddf460a33da2c/src/NetCoreKit.Utils/Helpers/EnumHelper.cs
    /// </summary>
    public static class EnumHelper
    {
        public static IEnumerable<KeyValuePair<TKey, string>> GetEnumKeyValue<TEnum, TKey>()
            where TKey : class
        {
            var metas = GetMetadata<TEnum, TKey>();
            var results = metas.Item1.Zip(metas.Item2, (key, value) =>
                new KeyValuePair<TKey, string>
                (
                    key,
                    value
                )
            );
            return results;
        }

        public static (IEnumerable<TKey>, IEnumerable<string>) GetMetadata<TEnum, TKey>()
        {
            var keyArray = (TKey[])Enum.GetValues(typeof(TEnum));
            var nameArray = Enum.GetNames(typeof(TEnum));

            IList<TKey> keys = new List<TKey>();
            foreach (var item in keyArray) keys.Add(item);

            IList<string> names = new List<string>();
            foreach (var item in nameArray) names.Add(item);

            return (keys, names);
        }
    }
}