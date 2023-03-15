using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Meadow.Configuration;
using Meadow.Contracts;

namespace Example.EventSourcing.Meadow;

public class MeadowConfigurationProvider : IMeadowConfigurationProvider
{
    public MeadowConfiguration GetConfigurations()
    {
        var saPassword = File.ReadAllText("sa.pass");
        
        
        return new MeadowConfiguration
        {
            
            
            ConnectionString = "Server=localhost;" +
                               "User Id=sa; " +
                               $"Password={saPassword};" +
                               $@"Database=MeadowExample; " +
                               "MultipleActiveResultSets=true",
            MacroPolicy = MacroPolicies.Ignore,
            BuildupScriptDirectory = "Scripts",
            MacroContainingAssemblies = new List<Assembly>
            {
                typeof(MeadowConfigurationProvider).Assembly
            },
            DatabaseFieldNameDelimiter = '_'
        };
    }
}