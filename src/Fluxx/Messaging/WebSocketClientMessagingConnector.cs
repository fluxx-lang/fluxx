using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Fluxx.Messaging
{
    public class WebSocketClientMessagingConnector : WebSocketMessagingConnector
    {
        private readonly string developmentMachine;

        public WebSocketClientMessagingConnector(string developmentMachine)
        {
            this.developmentMachine = developmentMachine;
        }

        public async Task Start()
        {

            try
            {
                var clientWebSocket = new ClientWebSocket();
                await clientWebSocket.ConnectAsync(new Uri("ws://" + this.developmentMachine), this.StopCts.Token);

                await this.ProcessMessages(clientWebSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // TODO: Add reconnect logic
            }

#if false
            try {
                await Task.Factory.StartNew(
                    async () => { await ProcessMessages(clientWebSocket); },
                    _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch(Exception e) {
                Console.WriteLine(e);
                // TODO: Add reconnect logic
            }
#endif
        }
    }
}
