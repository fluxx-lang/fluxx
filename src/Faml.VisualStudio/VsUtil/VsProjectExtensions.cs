using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Faml.VisualStudio.VsUtil {
    public static class VsProjectExtensions {
        public static string GetFilePath(this IVsProject project) {
            if (project.GetMkDocument(VSConstants.VSITEMID_ROOT, out string projectFilePath) != VSConstants.S_OK)
                throw new InvalidOperationException("Error getting project file name");

            return projectFilePath;
        }
    }
}
