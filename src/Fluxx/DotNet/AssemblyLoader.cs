using Fluxx.ProjectTypes;

namespace Fluxx.DotNet
{
    public abstract class AssemblyLoader
    {
        public abstract LoadedAssembly? Load(DotNetAssembly dotNetAssembly, out string? errorMessage);
    }
}
