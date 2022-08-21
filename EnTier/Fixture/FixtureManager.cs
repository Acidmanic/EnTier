using System;

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
                Console.WriteLine(e);
            }
        }
    }
}