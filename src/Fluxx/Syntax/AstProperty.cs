namespace Fluxx.Syntax
{
    public class AstProperty
    {
        private readonly string name;

        public AstProperty(string name)
        {
            this.name = name;
        }

        public string Name => this.name;
    }
}