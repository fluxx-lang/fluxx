using System;
using Faml.Api;
using Faml.Lang;
using Faml.Messaging;
using ReactiveData;

namespace Faml.DevEnv {
    public class DevEnvToAppConnection {
        private readonly MessagingConnector? _messagingConnector;

        public DevEnvToAppConnection(MessagingConnector messagingConnector) {
            _messagingConnector = messagingConnector;
        }

        public void UpdateSource(string modulePath, string source) {
            MessageObject request = new MessageObject("updateSource");
            request.AddProperty("sourcePath", modulePath);
            request.AddProperty("source", source);

            _messagingConnector.SendRequest(request);
        }

        public IReactive<ExampleResult[]> VisualizeExample(QualifiableName moduleName, int exampleIndex) {
            MessageObject request = new MessageObject("visualizeExample");
            request.AddProperty("moduleName", moduleName.ToString());
            request.AddProperty("exampleIndex", exampleIndex);

            _messagingConnector.SendRequest(request);

            throw new NotImplementedException();

#if LATER
            ReactiveVar<ExampleResult[]> exampleResults = _visualizeExampleValues.AddValue(null, out long valueId);

            FireAndForget(PublishAppRequestAsync($"visualize-example/request/{valueId}", request));

            return exampleResults;
#endif
        }

#if LATER
        private void ProcessVisualizeExampleResponse(string valueIdString, byte[] payload) {
            long valueId = long.Parse(valueIdString);
            ReactiveVar<ExampleResult[]> exampleResults = _visualizeExampleValues.GetValue(valueId);

            if (payload == null) {
                exampleResults.Set(CreateExampleResults("null response"));
                return;
            }

            VisualizeExampleResponse response;
            using (var payloadStream = new MemoryStream(payload)) {
                var formatter = new BinaryFormatter();
                object responseObject = formatter.Deserialize(payloadStream);

                if (! (responseObject is VisualizeExampleResponse)) {
                    exampleResults.Set(CreateExampleResults("invalid response"));
                    return;
                }

                response = (VisualizeExampleResponse)responseObject;
            }

            exampleResults.Set(response.ExampleResults);
        }

        private ExampleResult[] CreateExampleResults(object value) {
            var exampleResults = new ExampleResult[1];
            exampleResults[0] = new ExampleResult() {
                Content = value
            };
            return exampleResults;
        }

        private async Task InitializeAsync() {
            try {
                var binding = new TcpBinding();
                string hostAddress = IPAddress.Loopback.ToString();
                var mqttClientFactory = new MqttClientFactory(hostAddress, binding);

                _client = await mqttClientFactory.CreateClientAsync(_configuration);

                string clientId = GetClientId();
                await _client.ConnectAsync(new MqttClientCredentials(clientId));


                string visualizeExampleResponseTopic = _appName + "/visualize-example/response/";
                string visualizeExampleResponseTopicFilter = visualizeExampleResponseTopic + "+";

                //await appSubscriber.SubscribeAsync(topicFilter, MqttQualityOfService.AtMostOnce).ConfigureAwait(continueOnCapturedContext: false);
                await _client.SubscribeAsync(visualizeExampleResponseTopicFilter, MqttQualityOfService.AtMostOnce);

                _client.MessageStream
                    .Subscribe((MqttApplicationMessage message) => {
                        string topic = message.Topic;

                        if (topic.StartsWith(visualizeExampleResponseTopic)) {
                            string valueId = message.Topic.Substring(visualizeExampleResponseTopic.Length);
                            ProcessVisualizeExampleResponse(valueId, message.Payload);
                        }
                    });

                //_stop.Wait();

                /*
                await appSubscriber.UnsubscribeAsync(renderResponseTopicFilter)
                    .ConfigureAwait(continueOnCapturedContext: false);

                appSubscriber.Dispose();
                */
            }
            catch (Exception e) {
                Debug.WriteLine("Error initializing MQTT client:");
                Debug.Write(e);
            }
        }
#endif
    }
}
