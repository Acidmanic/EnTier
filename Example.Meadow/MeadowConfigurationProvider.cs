using System.IO;
using System.Net;
using Meadow.Configuration;
using Meadow.Scaffolding.Contracts;

namespace Example.Meadow
{
    public class MeadowConfigurationProvider:IMeadowConfigurationProvider
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
            };
        }
    }
}