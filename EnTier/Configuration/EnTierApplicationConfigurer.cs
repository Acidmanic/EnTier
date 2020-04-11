using EnTier.Binding.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Configuration
{

    public interface IEnTierApplicationConfigurer
    {
        IEnTierApplicationConfigurer SetContext<T>();

        IEnTierApplicationConfigurer RegisterServices(Action<IDIRegisterer> expression);

    }
}
