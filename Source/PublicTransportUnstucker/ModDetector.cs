using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;

namespace PublicTransportUnstucker
{
    public class ModDetector
    {
        // inspiration: AlgernonCommons
        public static Assembly LoadAssembly(string assemblyName)
        {
            return PluginManager.instance.GetPluginsInfo()
                .Where(plugin => plugin.isEnabled)
                .SelectMany(plugin => plugin.GetAssemblies()
                    .Where(assembly => assembly.GetName().Name == assemblyName)
                ).FirstOrDefault();
        }
    }
}
