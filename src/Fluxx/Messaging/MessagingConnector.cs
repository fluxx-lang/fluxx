using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveData;

namespace Fluxx.Messaging
{
    public delegate Task<MessageObject?> MessageHandler(MessageObject request);
    public delegate TResponse ResponseConverter<out TResponse>(MessageObject response);

    public abstract class MessagingConnector
    {
        private long responseIdCounter = 1;
        private readonly Dictionary<string, MessageHandler> messageHandlers = new Dictionary<string, MessageHandler>();
        private readonly Dictionary<long, ResponseHandler> neededResponses = new Dictionary<long, ResponseHandler>();

        public void AddMessageHandler(string messageType, MessageHandler handler)
        {
            this.messageHandlers.Add(messageType, handler);
        }

        protected async Task HandleMessage(Message message)
        {
            string messageType = message.MessageType;

            if (!this.messageHandlers.TryGetValue(messageType, out MessageHandler handler))
            {
                throw new InvalidOperationException($"Unsupported message type: {messageType}");
            }

            MessageObject? response = await handler(message.MessageObject);

            if (response == null)
            {
                return;
            }

            if (!response.Type.EndsWith("Response"))
            {
                throw new InvalidOperationException($"Response message type {response.Type} does not have suffix 'Response'");
            }

            long responseId = message.ResponseId;
            if (responseId == 0)
            {
                throw new InvalidOperationException($"Response for {messageType} was returned, but there's no responseId to return it");
            }

            var responseMessage = new Message(response, responseId);

            this.SendMessage(responseMessage);
        }

        protected abstract void SendMessage(Message message);

        public void SendRequest(MessageObject request)
        {
            Message requestMessage = new Message(request);
            this.SendMessage(requestMessage);
        }

        public IReactive<TResponse> SendRequestNeedingResponse<TResponse>(MessageObject request, ResponseConverter<TResponse> responseConverter) where TResponse : class?
        {
            long responseId = this.responseIdCounter++;
            Message requestMessage = new Message(request, responseId);

            var responseHandler = new ResponseHandler<TResponse>(responseConverter);
            this.neededResponses.Add(responseId, responseHandler);

            this.SendMessage(requestMessage);

            return responseHandler.ReactiveResponse;
        }

        protected void HandleResponse(Message responseMessage)
        {
            long responseId = responseMessage.ResponseId;
            if (responseId == 0)
            {
                throw new InvalidOperationException($"{responseMessage.MessageType} response message does not contain a responseId");
            }

            if (!this.neededResponses.TryGetValue(responseId, out ResponseHandler responseHandler))
            {
                throw new InvalidOperationException($"{responseMessage.MessageType} response message does not have a registered response handler for responseId {responseId}");
            }

            this.neededResponses.Remove(responseId);
            responseHandler.ResponseReceived(responseMessage);
        }

        private abstract class ResponseHandler
        {
            public abstract void ResponseReceived(Message responseMessage);
        }

        private class ResponseHandler<TResponse> : ResponseHandler where TResponse : class?
        {
            private readonly ResponseConverter<TResponse> responseConverter;
            public ReactiveVar<TResponse> ReactiveResponse { get; } = new ReactiveVar<TResponse>(null);

            public ResponseHandler(ResponseConverter<TResponse> responseConverter)
            {
                this.responseConverter = responseConverter;
            }

            public override void ResponseReceived(Message responseMessage)
            {
                TResponse response = this.responseConverter(responseMessage.MessageObject);
                this.ReactiveResponse.Set(response);
            }
        }
    }
}
