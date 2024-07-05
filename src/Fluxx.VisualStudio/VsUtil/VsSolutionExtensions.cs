using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Faml.VisualStudio.VsUtil {
    public static class VsSolutionExtensions {
        public static Guid GetProjectGuidInSolution(this IVsHierarchy hierarchy) {
            var vsSolution = FamlPackage.Instance.VsSolution;

            int retVal = vsSolution.GetGuidOfProject(hierarchy, out Guid projectGuidInSolution);
            if (retVal != VSConstants.S_OK)
                throw new InvalidOperationException($"Error {retVal} getting Guid for project");

            return projectGuidInSolution;
        }
    }
}
