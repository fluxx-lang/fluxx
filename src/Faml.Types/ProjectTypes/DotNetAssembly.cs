namespace Faml.ProjectTypes
{
    public class DotNetAssembly : ExternalPackage {
        public FilePath Path { get; set; }

        public string Name  { get; set; }
    }
}