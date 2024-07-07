using System;
using System.IO;

namespace Fluxx.DevEnv
{
    public class DevEnvUtil {
        /// <summary>
        /// Given a FAML source file, go up the directory hierarchy searching for a directory that contains "project.faml".
        /// If found, that directory is returned as the project directory.  Otherwise, null is returned.
        /// </summary>
        /// <param name="famlFile">FAML source file</param>
        /// <returns>project root directory for the source file or null</returns>
        public static string? GetProjectRootDirectoryForFile(string famlFile) {
            string fileDirectory = Path.GetDirectoryName(famlFile);

            if (string.IsNullOrEmpty(fileDirectory))
                throw new Exception($"Not a valid file path: {famlFile}");

            string currDirectory = fileDirectory;
            do {
                if (File.Exists(Path.Combine(currDirectory, "project.faml")))
                    return currDirectory;

                currDirectory = Path.GetDirectoryName(currDirectory);
            } while (! string.IsNullOrEmpty(currDirectory));

            return null;
        }
    }
}
