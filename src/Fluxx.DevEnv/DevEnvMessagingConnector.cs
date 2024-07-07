using Fluxx.Messaging;

namespace Fluxx.DevEnv {
    public class DevEnvMessagingConnector : WebSocketServerMessagingConnector {
        public DevEnvMessagingConnector() : base(5311)
        {
        }
    }
}
