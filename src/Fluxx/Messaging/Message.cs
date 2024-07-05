using System;
using System.IO;

namespace Faml.Messaging {
    public class Message {
        // The 4 signature bytes are "bMsg" in ASCII, standing for binary message. As BinaryReader/Writer works in
        // little endian, the bytes are reversed here.
        public const int SignatureBytes = 0x67734d62;

        private readonly MessageObject _messageObject;
        public long ResponseId { get; }

        public static Message Read(Stream inputStream) {
            using var binaryReader = new BinaryReader(inputStream);

            int signature = binaryReader.ReadInt32();
            if (signature != SignatureBytes)
                throw new FormatException("Message doesn't start with expected 4 signature bytes");

            long responseId = binaryReader.ReadInt64();
            MessageObject messageObject = MessageObject.Read(binaryReader);
            return new Message(messageObject, responseId);
        }

        public Message(string type) {
            _messageObject = new MessageObject(type);
        }

        public Message(MessageObject messageObject) {
            _messageObject = messageObject;
        }

        public Message(MessageObject messageObject, long responseId) {
            _messageObject = messageObject;
            ResponseId = responseId;
        }

        public string MessageType => _messageObject.Type;

        public MessageObject MessageObject => _messageObject;

        public bool IsResponse => MessageType.EndsWith("Response");

        public T GetProperty<T>(string propertyName) where T : class => _messageObject.GetProperty<T>(propertyName);

        public void AddProperty(string propertyName, string value) {
            _messageObject.AddProperty(propertyName, value);
        }

        public void AddProperty(string propertyName, int value) {
            _messageObject.AddProperty(propertyName, value);
        }

        public void Write(Stream outputStream) {
            using var binaryWriter = new BinaryWriter(outputStream);

            binaryWriter.Write(SignatureBytes);
            binaryWriter.Write(ResponseId);
            _messageObject.Write(binaryWriter);
        }
    }
}
