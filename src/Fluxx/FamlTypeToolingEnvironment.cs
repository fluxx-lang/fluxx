using System.Globalization;
using TypeTooling;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace Faml {
    public class FamlTypeToolingEnvironment : TypeToolingEnvironment {
        private readonly FamlProject _project;

        public FamlTypeToolingEnvironment(FamlProject project) {
            _project = project;
        }

        public override TypeToolingType? GetType(RawType rawType) {
            return _project.GetTypeToolingType(rawType);
        }

        public override TypeToolingType GetRequiredType(RawType rawType) {
            TypeToolingType typeToolingType = GetType(rawType);
            if (typeToolingType == null)
                throw new UserViewableException($"Could not find TypeToolingType for {rawType}");
            return typeToolingType;
        }

        public override RawType? GetRawType(string rawTypeName) {
            return _project.GetTypeToolingRawType(rawTypeName);
        }

        public override RawType GetRequiredRawType(string rawTypeName) {
            RawType rawType = GetRawType(rawTypeName);
            if (rawType == null)
                throw new UserViewableException($"Could not find type {rawTypeName}");
            return rawType;
        }

        public override object Instantiate(RawType rawType, params object[] args) {
            return _project.Instantiate(rawType, args);
        }

        public override CultureInfo UICulture => _project.Workspace.UICulture;
    }
}
