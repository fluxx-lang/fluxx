using System.Threading;
using System.Threading.Tasks;
using Faml.Messaging;
using Faml.Tests.Shared;
using NUnit.Framework;
using ReactiveData;


namespace Faml.Tests.Messaging {
    public sealed class WebSocketTests : TestBase {
        [Test] public void TestWebSocket() {
            var serverConnector = new WebSocketServerMessagingConnector(5555);
            var clientConnector = new WebSocketClientMessagingConnector("localhost:5555");

            IReactive<TestResponse> testResponse;
            try {
                clientConnector.AddMessageHandler("TestRequest", TestRequestHandler);
                serverConnector.Start();
                clientConnector.Start();

                MessageObject request = new MessageObject("TestRequest");
                request.AddProperty("intProperty", 3);

                testResponse = serverConnector.SendRequestNeedingResponse(request, TestResponseConverter);

                while (testResponse.Value == null)
                    Thread.Sleep(500);
            }
            finally {
                clientConnector.Stop();
                serverConnector.Stop();
            }

            Assert.AreEqual(true, testResponse.Value.Success);
        }

        private Task<MessageObject?> TestRequestHandler(MessageObject request) {
            MessageObject response = new MessageObject("TestResponse");
            response.AddProperty("intProperty", request.GetProperty<int>("intProperty"));

            return Task.FromResult(response);
        }

        private TestResponse TestResponseConverter(MessageObject response) {
            bool success = response.GetProperty<int>("intProperty") == 3;
            return new TestResponse(success);
        }

        class TestResponse {
            public bool Success { get; }

            public TestResponse(bool success) {
                Success = success;
            }
        }
    }
}
