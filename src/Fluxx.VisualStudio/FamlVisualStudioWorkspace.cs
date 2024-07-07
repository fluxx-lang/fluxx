using System;
using Fluxx.DevEnv;
using Fluxx.VisualStudio.VsUtil;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;

namespace Fluxx.VisualStudio {
    public class FamlVisualStudioWorkspace {
        /* private Dictionary<ITextBuffer, SourceBuffer> _sourceFileBuffers = new Dictionary<ITextBuffer, SourceBuffer>(); */
        private readonly DevEnvMessagingConnector _messagingConnector;
        private readonly DevEnvToAppConnection _appConnection;
        private readonly FamlWorkspace _famlWorkspace;
        private FamlVisualStudioProject _famlProject;   // For now, we just support a single project


        public FamlVisualStudioWorkspace(IComponentModel componentModel) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            _famlWorkspace = new FamlWorkspace();

            _messagingConnector = new DevEnvMessagingConnector();
            _messagingConnector.Start().ConfigureAwait(false);

            _appConnection = new DevEnvToAppConnection(_messagingConnector);

#if false
            // TODO: Start broker on demand
            _mqttBroker = new MqttBroker();
            _mqttBroker.Start();

            _devEnvMqttClient = new DevEnvMqttClient("myapp");
#endif
        }

        public DevEnvToAppConnection AppConnection => _appConnection;

        public FamlWorkspace FamlWorkspace => _famlWorkspace;

        public FamlVisualStudioProject FamlProject => _famlProject;

        public FamlVisualStudioProject GetSingleProject(IVsHierarchy hierarchy) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            Guid projectGuidInSolution = hierarchy.GetProjectGuidInSolution();

            if (_famlProject != null) {
                if (projectGuidInSolution != _famlProject.ProjectGuidInSolution)
                    throw new InvalidOperationException(
                        $"Currently, it's only supported having a single FAML project in a solution");
            }
            else
                _famlProject = new FamlVisualStudioProject(this, hierarchy, projectGuidInSolution);

            return _famlProject;
        }

        /*
        public void RecreateProgramIfNeeded(string mainSourceDirectory) {
            // If there's already a program, see if the project.faml file is unchanged (or doesn't exists).  In
            // that case, there's no need to recreate the program.
            if (_workspaceProxy != null) {
                string projectFile = Path.Combine(mainSourceDirectory, "project.faml");
            }

            // TODO: Recreate App Domain only when needed (when libraries list, or libraries themselves, change)
            if (_famlAppDomainHost != null)
                _famlAppDomainHost.Dispose();

            _famlAppDomainHost = new FamlAppDomainHost();

            //int tickCountStart = Environment.TickCount;

            try {
                _workspaceProxy = _famlAppDomainHost.CreateProgram(mainSourceDirectory);
                _workspaceProxy.RootProject.Initialize();
            }
            catch (Exception e) {
                //int elapsedTimeFailure = Environment.TickCount - tickCountStart;
                //Log(e.Message);
                //Log("Time elapsed (for failure): " + elapsedTimeFailure + "ms");
            }

            //int elapsedTime = Environment.TickCount - tickCountStart;
            //Log("Time elapsed (for success): " + elapsedTime + "ms");
        }
        */
    }
}
