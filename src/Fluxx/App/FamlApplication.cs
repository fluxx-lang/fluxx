using System;
using System.Collections.Generic;
using System.Reflection;
using Faml.DotNet;
using ReactiveData;

namespace Faml.App
{
    public abstract class FamlApplication
    {
        private ReactiveVar<Program>? _program;
        private AppToDevEnvConnection? _appToDevEnvConnection;

        protected void Initialize(Assembly assembly, string resourceFileName)
        {
            var workspace = new FamlWorkspace();

            var sourceProvider = new AssemblySourceProvider(assembly);
            FamlProject project = workspace.CreateProject(sourceProvider);

            project.DotNetProjectInfo.RawTypeProvider = new AppDotNetRawTypeProvider(assembly);

            var program = new Program(project)
            {
                DataEventHandler = new DotNetDataEventHandler(this.dataEventHandler)
            };

            this.AddTypeToolingProviders(program, project);

            project.FullyInitialize();

            // TODO: Fix to support an arbitrary number of files, etc.
            //project.LoadMainModule(new ModuleName(resourceFileName));
            string? mainSource = sourceProvider.GetTextResource(resourceFileName);
            if (mainSource == null)
            {
                throw new InvalidOperationException($"Source file {resourceFileName} not found");
            }

            project.UpdateSource(resourceFileName, mainSource);

            this._program = new ReactiveVar<Program>(program);
            this._appToDevEnvConnection = new AppToDevEnvConnection(this._program);
        }

        public AppToDevEnvConnection AppToDevEnvConnection
        {
            get
            {
                if (this._appToDevEnvConnection == null)
                {
                    throw new InvalidOperationException("AppToDevEnvConnection never got initialized");
                }

                return this._appToDevEnvConnection;
            }
        }

        public ReactiveVar<Program> Program
        {
            get
            {
                if (this._program == null)
                {
                    throw new InvalidOperationException("Program never got initialized");
                }

                return this._program;
            }
        }

        protected abstract void AddTypeToolingProviders(Program program, FamlProject project);

        public async void dataEventHandler(object sender, EventArgs e, IList<object> data)
        {
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

        public ReactiveVar<Program> GetProgram()
        {
            return this._program;
        }


    }
}
