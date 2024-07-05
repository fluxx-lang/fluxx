using Faml.Messaging;

namespace Faml.DevEnv {
    public class DevEnvMessagingConnector : WebSocketServerMessagingConnector {
        public DevEnvMessagingConnector() : base(5311)
        {
        }
    }
}
