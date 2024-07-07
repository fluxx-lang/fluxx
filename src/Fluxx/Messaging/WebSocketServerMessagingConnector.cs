using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Faml.Messaging
{
    public class WebSocketServerMessagingConnector : WebSocketMessagingConnector
    {
        private readonly HttpListener httpListener;

        public WebSocketServerMessagingConnector(int port)
        {
            this.httpListener = new HttpListener();
            this.httpListener.Prefixes.Add($"http://localhost:{port}/");
        }

        public async Task Start()
        {
            this.httpListener.Start();

            while (true)
            {
                HttpListenerContext listenerContext = await this.httpListener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    this.HandleWebSocketRequest(listenerContext).ConfigureAwait(false);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }

        //### Accepting WebSocket connections
        // Calling `AcceptWebSocketAsync` on the `HttpListenerContext` will accept the WebSocket connection, sending the required 101 response to the client
        // and return an instance of `WebSocketContext`. This class captures relevant information available at the time of the request and is a read-only 
        // type - you cannot perform any actual IO operations such as sending or receiving using the `WebSocketContext`. These operations can be 
        // performed by accessing the `System.Net.WebSocket` instance via the `WebSocketContext.WebSocket` property.        
        private async Task HandleWebSocketRequest(HttpListenerContext listenerContext)
        {
            WebSocketContext webSocketContext;
            try
            {
                webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
            }
            catch (Exception e)
            {
                // The upgrade process failed somehow. For simplicity lets assume it was a failure on the part of the server and indicate this using 500.
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Console.WriteLine("Exception: {0}", e);
                return;
            }

            using WebSocket webSocket = webSocketContext.WebSocket;

            try
            {
                await this.ProcessMessages(webSocket);
            }
            catch (Exception e)
            {
                // Just log any exceptions to the console. Pretty much any exception that occurs when calling `SendAsync`/`ReceiveAsync`/`CloseAsync` is unrecoverable in that it will abort the connection and leave the `WebSocket` instance in an unusable state.
                Console.WriteLine("Exception: {0}", e);
            }
        }
    }
}
