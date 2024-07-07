using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.CompanionType;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace Fluxx
{
    public sealed class FilePath
    {
        public string Path { get; }

        public FilePath(string path)
        {
            this.Path = path;
        }
    }

    public sealed class FilePathTypeTooling : ICustomLiteralParser
    {
        CustomLiteral ICustomLiteralParser.Parse(string value)
        {
            // TODO: FIX THIS UP - PASS TypeToolingEnvironment TO CONSTRUCTOR OR, MAYBE BETTER, SUPPORT PASSING Type HERE

            DotNetRawType filePathType = new ReflectionDotNetRawType(typeof(FilePath));

            NewObjectCode newFilePath =
                DotNetCode.New(filePathType, new[] { "System.String" }, Code.StringLiteral(value));
            //new FilePath(value);

            return new CustomLiteral(newFilePath);
        }
    }
}
