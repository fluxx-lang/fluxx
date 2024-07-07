using System;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */
namespace Faml.Api
{
    [Serializable]
    public struct Name : IEquatable<Name>
    {
        private readonly string name;

        public Name(string name)
        {
            this.name = name;
        }

        public QualifiableName ToQualifiableName() => new QualifiableName(this.name);

        public string GetPascalCase() => Util.Util.UpperCaseFirstCharacter(this.name);

        public override string ToString() => this.name;

        public string AsString() => this.name;

        public bool Equals(Name other) => string.Equals(this.name, other.name);

        public override bool Equals(object obj) => obj is Name && this.Equals((Name)obj);

        public static bool operator ==(Name val1, Name val2) => val1.Equals(val2);

        public static bool operator !=(Name val1, Name val2) => !val1.Equals(val2);

        public override int GetHashCode() => this.name.GetHashCode();
    }
}
