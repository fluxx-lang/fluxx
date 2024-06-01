

/**
 * @author Bret Johnson
 * @since 6/28/2014 11:21 PM
 */
namespace TypeTooling.CodeGeneration.Operators {
    public class Operator {
        private readonly string _defaultStringRepresentation;

        public Operator(string defaultStringRepresentation) {
            _defaultStringRepresentation = defaultStringRepresentation;
        }

        public string DefaultStringRepresentation => _defaultStringRepresentation;

        public override string ToString() => DefaultStringRepresentation;
    }
}
