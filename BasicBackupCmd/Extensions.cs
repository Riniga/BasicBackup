using System;
using System.IO;
using System.Reflection;

namespace BasicBackup
{
    public static class Extensions
    {
        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            return File.GetLastWriteTime(filePath);
        }
    }
}
