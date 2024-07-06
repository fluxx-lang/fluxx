namespace Faml.ProjectTypes
{
    public class Project
    {
        public DotNetAssembly Sdk { get; set; }

        public List<DotNetAssembly> Dependencies { get; set; } = new List<DotNetAssembly>();
    }
}
