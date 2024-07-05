using System;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */

namespace Faml.Api {
    [Serializable]
    public struct Name : IEquatable<Name> {
        private readonly string _name;

        public Name(string name) {
            _name = name;
        }

        public QualifiableName ToQualifiableName() => new QualifiableName(_name);

        public string GetPascalCase() => Util.Util.UpperCaseFirstCharacter(_name);

        public override string ToString() => _name;

        public string AsString() => _name;

        public bool Equals(Name other) => string.Equals(_name, other._name);

        public override bool Equals(object obj) => obj is Name && Equals((Name) obj);

        public static bool operator ==(Name val1, Name val2) => val1.Equals(val2);

        public static bool operator !=(Name val1, Name val2) => ! val1.Equals(val2);

        public override int GetHashCode() => _name.GetHashCode();
    }
}
