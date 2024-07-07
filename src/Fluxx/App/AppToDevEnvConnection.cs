using System;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Lang;
using Faml.Messaging;
using ReactiveData;
using TypeTooling;

namespace Faml.App {
    public class AppToDevEnvConnection {
        private MessagingConnector? _connector;
        private readonly ReactiveVar<Program> _program;
        private IVisualizer? _visualizer;


        public AppToDevEnvConnection(ReactiveVar<Program> program) {
            this._program = program;
        }

        public void Start() {
            AppProjectInfo? appProjectInfo = this._program.Value.RootProject.AppProjectInfo;
            string? developmentMachine = appProjectInfo?.DevelopmentMachine;
            if (developmentMachine == null)
                // developmentMachine = "10.0.2.2:5311";   // Use the Android emulator host IP for now
                developmentMachine = "localhost:5311";

            WebSocketClientMessagingConnector webSocketConnector = new WebSocketClientMessagingConnector(developmentMachine);
            webSocketConnector.AddMessageHandler("updateSource", this.UpdateSourceHandler);
            webSocketConnector.AddMessageHandler("visualizeExample", this.VisualizeExampleHandler);

            this._connector = webSocketConnector;
            webSocketConnector.Start().ConfigureAwait(false);
        }

        public void SetVisualizer(IVisualizer visualizer) {
            this._visualizer = visualizer;
        }

        private Task<MessageObject?> UpdateSourceHandler(MessageObject request) {
            string sourcePath = request.GetProperty<string>("sourcePath");
            string source = request.GetProperty<string>("source");

            // Lock to avoid multiple updates happening at the same time
            lock (this._program) {
                Transaction.Start();
                this._program.Value.RootProject.UpdateSource(sourcePath, source);
                this._program.NotifyChanged();
                Transaction.End();
            }

            return Task.FromResult((MessageObject?) null);
        }

        private async Task<MessageObject?> VisualizeExampleHandler(MessageObject request) {
            string moduleNameString = request.GetProperty<string>("moduleName");
            int exampleIndex = request.GetProperty<int>("exampleIndex");

            if (this._visualizer == null)
                throw new UserViewableException("No Visualizer is set on the app");

            ExampleResult[] visualizedExamples = await this._visualizer?.VisualizeExample(new QualifiableName(moduleNameString), exampleIndex);
            throw new NotImplementedException("TODO: Finish visual example");
        }
    }
}
