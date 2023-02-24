using System.IO;
using System.Reflection;

namespace EnTier.Utility;

public class SpecialPaths
{
    public static string GetExecutionDirectory()
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        
        var assemblyLocation = new FileInfo(assembly.Location)
            .Directory?.FullName ?? new DirectoryInfo(".").FullName;

        return assemblyLocation;
    }
}