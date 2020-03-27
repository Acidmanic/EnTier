using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration
{

    public interface IEnTierApplicationConfigurer
    {
        IEnTierApplicationConfigurer SetContext<T>();
    }
    internal class EnTierApplicationConfigurer: IEnTierApplicationConfigurer
    {
        public IEnTierApplicationConfigurer SetContext<T>()
        {

            EnTierApplication.ContextType = typeof(T);

            EnTierApplication.UseConfiguredContextType = true;

            return this;
        }
    }
}
