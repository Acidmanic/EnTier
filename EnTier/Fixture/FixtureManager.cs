using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EnTier.Fixture
{
    public static class FixtureManager
    {

        public static void UseFixture<TFixture>(EnTierEssence essence)
        {
            var executer = new FixtureExecuter(essence);
            
            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                essence.Logger.LogError(e,"Problem executing Fixture. {Error}",e);
            }
        }
    }
}