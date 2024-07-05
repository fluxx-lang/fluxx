using System;
using System.IO;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */

namespace Faml.Api {
    [Serializable]
    public struct QualifiableName : IEquatable<QualifiableName> {
        private readonly string _name;

        public static readonly QualifiableName EmptyName = new QualifiableName("");

        public static QualifiableName ModuleNameFromRelativePath(string path) {
            string extension = Path.GetExtension(path);

            string pathNoExtension = extension.Length > 0 ? path.Substring(0, path.Length - extension.Length) : path;
            string moduleNameString = pathNoExtension.Replace(Path.DirectorySeparatorChar, '.');
            return new QualifiableName(moduleNameString);
        }

        public QualifiableName(string name) {
            _name = name;
        }

        public QualifiableName(QualifiableName qualifier, Name lastComponent) {
            _name = qualifier + "." + lastComponent;
        }

        public bool IsQualified() => _name.IndexOf('.') != -1;

        public bool IsEmpty() => _name.Length == 0;

        public Name ToUnqualifiableName() {
            if (IsQualified())
                throw new Exception($"Can't convert qualified name '{this}' to unqualified name");

            return new Name(_name);
        }

        public string ToRelativePath() {
            return _name.Replace('.', Path.PathSeparator) + ".faml";
        }

        public Name GetLastComponent() {
            int lastPeriod = _name.LastIndexOf('.');
            if (lastPeriod == -1)
                return new Name(_name);
            else return new Name(_name.Substring(lastPeriod + 1));
        }

        /// <summary>
        /// See if the specified name is the last component of this qualfiable name. Also returns true if the names are the same
        /// (this name doesn't have any qualifiers).
        /// </summary>
        /// <param name="unqualifiedName">unqualified name to match against</param>
        /// <returns>true if unqualifiedName is the last component of this name or the same as this name</returns>
        public bool LastComponentMatches(string unqualifiedName) {
            int lastComponentIndex = _name.Length - unqualifiedName.Length;
            return _name.EndsWith(unqualifiedName) && lastComponentIndex == 0 || (lastComponentIndex > 0 && _name[lastComponentIndex - 1] == '.');
        }

        public bool LastComponentMatches(Name unqualifiedName) {
            return LastComponentMatches(unqualifiedName.ToString());
        }

        public QualifiableName GetQualifier() {
            int lastPeriodIndex = _name.LastIndexOf('.');
            if (lastPeriodIndex == -1)
                throw new Exception($"Can't call GetQualifier on unqualified name: {_name}");
            return new QualifiableName(_name.Substring(0, lastPeriodIndex));
        }

        public override string ToString() {
            return _name;
        }

        public bool Equals(QualifiableName other) {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj) {
            return obj is Name && Equals((Name) obj);
        }

        public static bool operator ==(QualifiableName val1, QualifiableName val2) {
            return val1.Equals(val2);
        }

        public static bool operator !=(QualifiableName val1, QualifiableName val2) {
            return !val1.Equals(val2);
        }

        public override int GetHashCode() {
            return _name.GetHashCode();
        }
    }
}
