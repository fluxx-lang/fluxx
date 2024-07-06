

/**
 * @author Bret Johnson
 * @since 6/28/2014 11:21 PM
 */
namespace TypeTooling.CodeGeneration.Operators
{
    public class Operator(string defaultStringRepresentation)
    {
        public string DefaultStringRepresentation => defaultStringRepresentation;

        public override string ToString() => this.DefaultStringRepresentation;
    }
}
