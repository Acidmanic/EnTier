using System.Reflection;

namespace EnTier.EventStore.WebView
{
    public static class AssemblyEventStoreWebViewExtensions
    {


        public static Assembly Scan(this Assembly assembly)
        {
            TypeRepository.Instance.Scan(assembly);

            return assembly;
        }
        
    }
}