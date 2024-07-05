namespace Faml.Syntax {
    public class AstProperty {
        private readonly string _name;

        public AstProperty(string name) {
            _name = name;
        }

        public string Name => _name;
    }
}