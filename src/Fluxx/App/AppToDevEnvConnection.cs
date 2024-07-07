using System;
using System.Threading.Tasks;
using Fluxx.Api;
using Fluxx.Lang;
using Fluxx.Messaging;
using ReactiveData;
using TypeTooling;

namespace Fluxx.App
{
    public class AppToDevEnvConnection
    {
        private MessagingConnector? connector;
        private readonly ReactiveVar<Program> program;
        private IVisualizer? visualizer;

        public AppToDevEnvConnection(ReactiveVar<Program> program)
        {
            this.program = program;
        }

        public void Start()
        {
            AppProjectInfo? appProjectInfo = this.program.Value.RootProject.AppProjectInfo;
            string? developmentMachine = appProjectInfo?.DevelopmentMachine;
            if (developmentMachine == null)
            {
                // developmentMachine = "10.0.2.2:5311";   // Use the Android emulator host IP for now
                developmentMachine = "localhost:5311";
            }

            WebSocketClientMessagingConnector webSocketConnector = new WebSocketClientMessagingConnector(developmentMachine);
            webSocketConnector.AddMessageHandler("updateSource", this.UpdateSourceHandler);
            webSocketConnector.AddMessageHandler("visualizeExample", this.VisualizeExampleHandler);

            this.connector = webSocketConnector;
            webSocketConnector.Start().ConfigureAwait(false);
        }

        public void SetVisualizer(IVisualizer visualizer)
        {
            this.visualizer = visualizer;
        }

        private Task<MessageObject?> UpdateSourceHandler(MessageObject request)
        {
            string sourcePath = request.GetProperty<string>("sourcePath");
            string source = request.GetProperty<string>("source");

            // Lock to avoid multiple updates happening at the same time
            lock (this.program)
            {
                Transaction.Start();
                this.program.Value.RootProject.UpdateSource(sourcePath, source);
                this.program.NotifyChanged();
                Transaction.End();
            }

            return Task.FromResult((MessageObject?)null);
        }

        private async Task<MessageObject?> VisualizeExampleHandler(MessageObject request)
        {
            string moduleNameString = request.GetProperty<string>("moduleName");
            int exampleIndex = request.GetProperty<int>("exampleIndex");

            if (this.visualizer == null)
            {
                throw new UserViewableException("No Visualizer is set on the app");
            }

            ExampleResult[] visualizedExamples = await this.visualizer?.VisualizeExample(new QualifiableName(moduleNameString), exampleIndex);
            throw new NotImplementedException("TODO: Finish visual example");
        }
    }
}
