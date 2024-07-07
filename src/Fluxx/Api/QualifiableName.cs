using System;
using System.IO;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */

namespace Faml.Api
{
    [Serializable]
    public struct QualifiableName : IEquatable<QualifiableName>
    {
        private readonly string name;

        public static readonly QualifiableName EmptyName = new QualifiableName("");

        public static QualifiableName ModuleNameFromRelativePath(string path)
        {
            string extension = Path.GetExtension(path);

            string pathNoExtension = extension.Length > 0 ? path.Substring(0, path.Length - extension.Length) : path;
            string moduleNameString = pathNoExtension.Replace(Path.DirectorySeparatorChar, '.');
            return new QualifiableName(moduleNameString);
        }

        public QualifiableName(string name)
        {
            this.name = name;
        }

        public QualifiableName(QualifiableName qualifier, Name lastComponent)
        {
            this.name = qualifier + "." + lastComponent;
        }

        public bool IsQualified() => this.name.IndexOf('.') != -1;

        public bool IsEmpty() => this.name.Length == 0;

        public Name ToUnqualifiableName()
        {
            if (this.IsQualified())
            {
                throw new Exception($"Can't convert qualified name '{this}' to unqualified name");
            }

            return new Name(this.name);
        }

        public string ToRelativePath()
        {
            return this.name.Replace('.', Path.PathSeparator) + ".faml";
        }

        public Name GetLastComponent()
        {
            int lastPeriod = this.name.LastIndexOf('.');
            if (lastPeriod == -1)
            {
                return new Name(this.name);
            }
            else
            {
                return new Name(this.name.Substring(lastPeriod + 1));
            }
        }

        /// <summary>
        /// See if the specified name is the last component of this qualfiable name. Also returns true if the names are the same
        /// (this name doesn't have any qualifiers).
        /// </summary>
        /// <param name="unqualifiedName">unqualified name to match against</param>
        /// <returns>true if unqualifiedName is the last component of this name or the same as this name</returns>
        public bool LastComponentMatches(string unqualifiedName)
        {
            int lastComponentIndex = this.name.Length - unqualifiedName.Length;
            return this.name.EndsWith(unqualifiedName) && lastComponentIndex == 0 || (lastComponentIndex > 0 && this.name[lastComponentIndex - 1] == '.');
        }

        public bool LastComponentMatches(Name unqualifiedName)
        {
            return this.LastComponentMatches(unqualifiedName.ToString());
        }

        public QualifiableName GetQualifier()
        {
            int lastPeriodIndex = this.name.LastIndexOf('.');
            if (lastPeriodIndex == -1)
            {
                throw new Exception($"Can't call GetQualifier on unqualified name: {this.name}");
            }

            return new QualifiableName(this.name.Substring(0, lastPeriodIndex));
        }

        public override string ToString()
        {
            return this.name;
        }

        public bool Equals(QualifiableName other)
        {
            return string.Equals(this.name, other.name);
        }

        public override bool Equals(object obj)
        {
            return obj is Name && this.Equals((Name)obj);
        }

        public static bool operator ==(QualifiableName val1, QualifiableName val2)
        {
            return val1.Equals(val2);
        }

        public static bool operator !=(QualifiableName val1, QualifiableName val2)
        {
            return !val1.Equals(val2);
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }
    }
}
