using System;
using System.Collections.Generic;
using System.Reflection;
using Faml.DotNet;
using ReactiveData;

namespace Faml.App {
    public abstract class FamlApplication {
        private ReactiveVar<Program>? _program;
        private AppToDevEnvConnection? _appToDevEnvConnection;

        protected void Initialize(Assembly assembly, string resourceFileName) {
            var workspace = new FamlWorkspace();

            var sourceProvider = new AssemblySourceProvider(assembly);
            FamlProject project = workspace.CreateProject(sourceProvider);

            project.DotNetProjectInfo.RawTypeProvider = new AppDotNetRawTypeProvider(assembly);

            var program = new Program(project) {
                DataEventHandler = new DotNetDataEventHandler(dataEventHandler)
            };

            AddTypeToolingProviders(program, project);

            project.FullyInitialize();

            // TODO: Fix to support an arbitrary number of files, etc.
            //project.LoadMainModule(new ModuleName(resourceFileName));
            string? mainSource = sourceProvider.GetTextResource(resourceFileName);
            if (mainSource == null)
                throw new InvalidOperationException($"Source file {resourceFileName} not found");

            project.UpdateSource(resourceFileName, mainSource);

            _program = new ReactiveVar<Program>(program);
            _appToDevEnvConnection = new AppToDevEnvConnection(_program);
        }

        public AppToDevEnvConnection AppToDevEnvConnection {
            get {
                if (_appToDevEnvConnection == null)
                    throw new InvalidOperationException("AppToDevEnvConnection never got initialized");
                return _appToDevEnvConnection;
            }
        }

        public ReactiveVar<Program> Program {
            get {
                if (_program == null)
                    throw new InvalidOperationException("Program never got initialized");
                return _program;
            }
        }

        protected abstract void AddTypeToolingProviders(Program program, FamlProject project);

        public async void dataEventHandler(object sender, EventArgs e, IList<object> data) {
            /*^
             foreach (object dataItem in data) {
                 if (dataItem is Navigate) {
                     var navigate = ((Navigate) dataItem);

                     Page page = navigate.Content;
                     await _application.MainPage.Navigation.PushAsync(page);
                 }
             }

            foreach (object dataItem in data) {
                if (dataItem is Navigate navigate)
                    await NavigateTo(navigate.Content);
            }
             */
        }

        public ReactiveVar<Program> GetProgram() {
            return _program;
        }


    }
}
