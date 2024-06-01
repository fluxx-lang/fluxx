using System;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Messaging;
using Faml.Lang;
using ReactiveData;
using TypeTooling;

namespace Faml.App {
    public class AppToDevEnvConnection {
        private MessagingConnector? _connector;
        private readonly ReactiveVar<Program> _program;
        private IVisualizer? _visualizer;


        public AppToDevEnvConnection(ReactiveVar<Program> program) {
            _program = program;
        }

        public void Start() {
            AppProjectInfo? appProjectInfo = _program.Value.RootProject.AppProjectInfo;
            string? developmentMachine = appProjectInfo?.DevelopmentMachine;
            if (developmentMachine == null)
                // developmentMachine = "10.0.2.2:5311";   // Use the Android emulator host IP for now
                developmentMachine = "localhost:5311";

            WebSocketClientMessagingConnector webSocketConnector = new WebSocketClientMessagingConnector(developmentMachine);
            webSocketConnector.AddMessageHandler("updateSource", UpdateSourceHandler);
            webSocketConnector.AddMessageHandler("visualizeExample", VisualizeExampleHandler);

            _connector = webSocketConnector;
            webSocketConnector.Start().ConfigureAwait(false);
        }

        public void SetVisualizer(IVisualizer visualizer) {
            _visualizer = visualizer;
        }

        private Task<MessageObject?> UpdateSourceHandler(MessageObject request) {
            string sourcePath = request.GetProperty<string>("sourcePath");
            string source = request.GetProperty<string>("source");

            // Lock to avoid multiple updates happening at the same time
            lock (_program) {
                Transaction.Start();
                _program.Value.RootProject.UpdateSource(sourcePath, source);
                _program.NotifyChanged();
                Transaction.End();
            }

            return Task.FromResult((MessageObject?) null);
        }

        private async Task<MessageObject?> VisualizeExampleHandler(MessageObject request) {
            string moduleNameString = request.GetProperty<string>("moduleName");
            int exampleIndex = request.GetProperty<int>("exampleIndex");

            if (_visualizer == null)
                throw new UserViewableException("No Visualizer is set on the app");

            ExampleResult[] visualizedExamples = await _visualizer?.VisualizeExample(new QualifiableName(moduleNameString), exampleIndex);
            throw new NotImplementedException("TODO: Finish visual example");
        }
    }
}
