using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Faml.Messaging {
    public class WebSocketClientMessagingConnector : WebSocketMessagingConnector {
        private readonly string _developmentMachine;


        public WebSocketClientMessagingConnector(string developmentMachine) {
            this._developmentMachine = developmentMachine;
        }

        public async Task Start() {

            try {
                var clientWebSocket = new ClientWebSocket();
                await clientWebSocket.ConnectAsync(new Uri("ws://" + this._developmentMachine), this.StopCts.Token);

                await this.ProcessMessages(clientWebSocket);
            }
            catch (Exception e) {
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
