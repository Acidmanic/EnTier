using System.IO;
using System.Reflection;

namespace EnTier.EventStore.WebView
{
    public static class ObjectSpecialDirectoryExtensions
    {
        public static string GetAssemblyDirectory(this object me)
        {
            var assemblyDirectory = Assembly.GetEntryAssembly()?.Location;

            if (assemblyDirectory != null)
            {
                assemblyDirectory = new FileInfo(assemblyDirectory).Directory?.FullName;
            }

            var filePath = assemblyDirectory ?? ".";

            filePath = new DirectoryInfo(filePath).FullName;

            return filePath;
        }
    }
}