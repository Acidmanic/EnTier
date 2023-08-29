using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Macros;
using Meadow.SqlServer;

namespace Example.Meadow.Configurations
{
    public class SqlServerConfigurationProvider:IMeadowConfigurationProvider
    {
                
        public MeadowConfiguration GetConfigurations()
        {

            var saPassword = File.ReadAllText("sa.pass");
            
            return new MeadowConfiguration
            {
                BuildupScriptDirectory = "Scripts",
                ConnectionString = "Server=localhost;" +
                                   "User Id=sa; " +
                                   $"Password={saPassword};" +
                                   $@"Database=MeadowExample; " +
                                   "MultipleActiveResultSets=true",
                MacroPolicy = MacroPolicies.UpdateScripts,
                MacroContainingAssemblies = new List<Assembly>
                {
                    Assembly.GetEntryAssembly(),
                    typeof(IMacro).Assembly,
                    typeof(SqlServerDataAccessCore).Assembly
                }
            };
        }
    }
}