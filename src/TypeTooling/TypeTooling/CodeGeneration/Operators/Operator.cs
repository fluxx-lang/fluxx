

/**
 * @author Bret Johnson
 * @since 6/28/2014 11:21 PM
 */
namespace TypeTooling.CodeGeneration.Operators
{
    public class Operator
    {
        private readonly string _defaultStringRepresentation;

        public Operator(string defaultStringRepresentation)
        {
            this._defaultStringRepresentation = defaultStringRepresentation;
        }

        public string DefaultStringRepresentation => this._defaultStringRepresentation;

        public override string ToString() => this.DefaultStringRepresentation;
    }
}
