namespace Faml.Syntax {
    public class AstProperty {
        private readonly string _name;

        public AstProperty(string name) {
            this._name = name;
        }

        public string Name => this._name;
    }
}