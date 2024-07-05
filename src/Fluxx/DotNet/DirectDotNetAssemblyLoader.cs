using System;
using System.Reflection;
using Faml.ProjectTypes;

/**
 * @author Bret Johnson
 * @since 3/28/2015
 */
namespace Faml.DotNet {
    public class DirectDotNetAssemblyLoader : AssemblyLoader {
        public override LoadedAssembly? Load(DotNetAssembly dotNetAssembly, out string? errorMessage) {
            errorMessage = null;

            // TODO: Hack this for now
            if (dotNetAssembly.Name == "Windows")
                return new LoadedAssembly(null, "Windows, ContentType = WindowsRuntime");

            var assemblyName = new AssemblyName(dotNetAssembly.Name);

            try {
                Assembly assembly = Assembly.Load(assemblyName);
                return new LoadedAssembly(assembly);
            }
            catch (Exception e) {
                errorMessage = $"Error occurred loading library {dotNetAssembly.Name}: {e.Message}";
                return null;
            }
        }
    }
}
