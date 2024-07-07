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
            this._name = name;
        }

        public QualifiableName ToQualifiableName() => new QualifiableName(this._name);

        public string GetPascalCase() => Util.Util.UpperCaseFirstCharacter(this._name);

        public override string ToString() => this._name;

        public string AsString() => this._name;

        public bool Equals(Name other) => string.Equals(this._name, other._name);

        public override bool Equals(object obj) => obj is Name && this.Equals((Name) obj);

        public static bool operator ==(Name val1, Name val2) => val1.Equals(val2);

        public static bool operator !=(Name val1, Name val2) => ! val1.Equals(val2);

        public override int GetHashCode() => this._name.GetHashCode();
    }
}
