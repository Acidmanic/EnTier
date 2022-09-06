using System;
using EnTier.Logging;
using Microsoft.Extensions.Logging;

namespace EnTier.Fixture
{
    public static class FixtureManager
    {
        public static void UseFixture<TFixture>(IFixtureResolver resolver)
        {
            var executer = new FixtureExecuter(resolver);

            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                EnTierLogging.GetInstance().Logger.LogError(e,"Problem executing Fixture.");
            }
        }
    }
}