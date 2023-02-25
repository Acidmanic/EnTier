using System.IO;
using Meadow.Configuration;
using Meadow.Contracts;

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