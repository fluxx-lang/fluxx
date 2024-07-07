using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Faml.Messaging {
    public class WebSocketMessagingConnector : MessagingConnector {
        private readonly AsyncQueue<Message> _sendMessageQueue = new AsyncQueue<Message>();
        private readonly CancellationTokenSource _stopCts;
        private readonly CancellationTokenSource _sendCts;
        private readonly CancellationTokenSource _receiveCts;


        public WebSocketMessagingConnector() {
            this._stopCts = new CancellationTokenSource();
            this._sendCts = new CancellationTokenSource();
            this._receiveCts = new CancellationTokenSource();
        }

        public virtual void Stop() {
            this._stopCts.Cancel();
        }

        public CancellationTokenSource StopCts => this._stopCts;

        protected override void SendMessage(Message message) {
            this._sendMessageQueue.Enqueue(message);
        }

        protected async Task ProcessMessages(WebSocket webSocket) {
            // While the socket stays open we receive everything we can & send everything we can, until someone
            // cancels or disconnects the socket. The WebSockets API allows at most 1 receive and 1 send operation
            // to be going on at the same time, which we observe
            Task whenAll = Task.WhenAll(this.ReceiveMessagesAsync(webSocket), this.SendMessagesAsync(webSocket));
            await whenAll;
        }

        private async Task ReceiveMessagesAsync(WebSocket webSocket)
        {
            try {
                var receiveAndOverallCts = CancellationTokenSource.CreateLinkedTokenSource(this._receiveCts.Token, this._stopCts.Token);

                var buffer = new ArraySegment<byte>(new byte[4096]);
                using var memoryStream = new MemoryStream();

                while (webSocket.State == WebSocketState.Open) {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, receiveAndOverallCts.Token);

                    if (result.MessageType == WebSocketMessageType.Close) {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", receiveAndOverallCts.Token);
                        return;
                    }

                    if (result.MessageType != WebSocketMessageType.Binary) {
                        await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept text frame",
                            receiveAndOverallCts.Token);
                        throw new InvalidOperationException(
                            $"Unexpected web socket message type received: {result.MessageType}");
                    }

                    Message message;
                    if (result.EndOfMessage) {
                        // Optimize for the typical case where there's just a single message
                        message = Message.Read(new MemoryStream(buffer.Array, buffer.Offset, buffer.Count));
                    }
                    else {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.Write(buffer.Array, buffer.Offset, result.Count);

                        do {
                            result = await webSocket.ReceiveAsync(buffer, receiveAndOverallCts.Token);
                            memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                        } while (!result.EndOfMessage);

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        message = Message.Read(memoryStream);
                    }

                    if (message.IsResponse)
                    {
                        this.HandleResponse(message);
                    }
                    else
                    {
                        await this.HandleMessage(message);
                    }
                }
            }
            catch (Exception e) {
                // If the receive fails, then also cancel the send
                if (!(e is OperationCanceledException))
                {
                    this._sendCts.Cancel();
                }

                throw;
            }
        }

        private async Task SendMessagesAsync(WebSocket webSocket) {
            try {
                var sendAndOverallCts = CancellationTokenSource.CreateLinkedTokenSource(this._sendCts.Token, this._stopCts.Token);

                while (webSocket.State == WebSocketState.Open) {
                    Message message = await this._sendMessageQueue.DequeueAsync(sendAndOverallCts.Token);

                    using var memoryStream = new MemoryStream();
                    message.Write(memoryStream);

                    if (!memoryStream.TryGetBuffer(out ArraySegment<byte> buffer))
                    {
                        throw new InvalidOperationException("TryGetBuffer failed");
                    }

                    await webSocket.SendAsync(buffer, WebSocketMessageType.Binary, true, sendAndOverallCts.Token);
                }
            }
            catch (Exception e) {
                // If the send fails then also cancel the receive
                if (! (e is OperationCanceledException))
                {
                    this._receiveCts.Cancel();
                }

                throw;
            }
        }
    }
}
