using Bootstrap.Starters;
using EnTier.Binding.Abstractions;
using EnTier.Bootstrap;
using EnTier.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Bootstrap.Starters
{
    public static class Scratch
    {



        public static void PerformRegistrations(IDIRegisterer registerer)
        {
            Boot.Strap(registerer);
        }

        public static void Start()
        {
            Start(c => { });
        }

        public static void Start(Action<IEnTierApplicationConfigurer> configurer)
        {
            var res = new DefaultResolver();

            Boot.Strap((IDIRegisterer)res);

            Start(configurer, res);
        }



        public static void Start(Action<IEnTierApplicationConfigurer> configurer, IDIResolver resolver, IDIRegisterer registerer)
        {
            Boot.Strap(registerer);

            Start(configurer,resolver);
        }

        public static void Start(Action<IEnTierApplicationConfigurer> configurer,IDIResolver resolver)
        {
            Boot.Strap(configurer);

            Boot.Strap(resolver);

            Boot.Go();
        }







    }
}
