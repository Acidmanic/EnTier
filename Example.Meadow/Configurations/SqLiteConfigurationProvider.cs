using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Macros;
using Meadow.SQLite;
using Meadow.SqlServer;

namespace Example.Meadow.Configurations
{
    public class SqLiteConfigurationProvider:IMeadowConfigurationProvider
    {
                
        public MeadowConfiguration GetConfigurations()
        {
            
            return new MeadowConfiguration
            {
                BuildupScriptDirectory = "Scripts",
                ConnectionString = "Data Source=MeadowExample.db",
                MacroPolicy = MacroPolicies.UpdateScripts,
                MacroContainingAssemblies = new List<Assembly>
                {
                    Assembly.GetEntryAssembly(),
                    typeof(IMacro).Assembly,
                    typeof(SqLiteDataAccessCore).Assembly
                }
            };
        }
    }
}