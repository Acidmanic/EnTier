using System.Reflection;

namespace EnTier.EventStore.WebView
{
    public static class AssemblyEventStoreWebViewExtensions
    {


        public static Assembly ScanForAggregates(this Assembly assembly)
        {
            TypeRepository.Instance.Scan(assembly);

            return assembly;
        }
        
    }
}