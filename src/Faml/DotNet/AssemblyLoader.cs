using Faml.ProjectTypes;

namespace Faml.DotNet {
    public abstract class AssemblyLoader {
        public abstract LoadedAssembly? Load(DotNetAssembly dotNetAssembly, out string? errorMessage);
    }
}
