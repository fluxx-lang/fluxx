using System.Globalization;
using TypeTooling;
using TypeTooling.RawTypes;
using TypeTooling.Types;

namespace Faml {
    public class FamlTypeToolingEnvironment : TypeToolingEnvironment {
        private readonly FamlProject _project;

        public FamlTypeToolingEnvironment(FamlProject project) {
            this._project = project;
        }

        public override TypeToolingType? GetType(RawType rawType) {
            return this._project.GetTypeToolingType(rawType);
        }

        public override TypeToolingType GetRequiredType(RawType rawType) {
            TypeToolingType typeToolingType = this.GetType(rawType);
            if (typeToolingType == null)
            {
                throw new UserViewableException($"Could not find TypeToolingType for {rawType}");
            }

            return typeToolingType;
        }

        public override RawType? GetRawType(string rawTypeName) {
            return this._project.GetTypeToolingRawType(rawTypeName);
        }

        public override RawType GetRequiredRawType(string rawTypeName) {
            RawType rawType = this.GetRawType(rawTypeName);
            if (rawType == null)
            {
                throw new UserViewableException($"Could not find type {rawTypeName}");
            }

            return rawType;
        }

        public override object Instantiate(RawType rawType, params object[] args) {
            return this._project.Instantiate(rawType, args);
        }

        public override CultureInfo UICulture => this._project.Workspace.UICulture;
    }
}
