

/**
 * @author Bret Johnson
 * @since 6/28/2014 11:21 PM
 */
namespace TypeTooling.CodeGeneration.Operators
{
    public class Operator
    {
        private readonly string defaultStringRepresentation;

        public Operator(string defaultStringRepresentation)
        {
            this.defaultStringRepresentation = defaultStringRepresentation;
        }

        public string DefaultStringRepresentation => this.defaultStringRepresentation;

        public override string ToString() => this.DefaultStringRepresentation;
    }
}
