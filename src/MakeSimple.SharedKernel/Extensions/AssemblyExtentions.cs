using System;
using System.IO;
using System.Reflection;

namespace MakeSimple.SharedKernel.Extensions
{
    public static class AssemblyExtentions
    {
        public static DateTime GetCreationTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var fi = new FileInfo(assembly.Location);
            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(fi.CreationTimeUtc, tz);

            return localTime;
        }
    }
}
