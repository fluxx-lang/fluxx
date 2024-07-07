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
            {
                throw new FormatException("Message doesn't start with expected 4 signature bytes");
            }

            long responseId = binaryReader.ReadInt64();
            MessageObject messageObject = MessageObject.Read(binaryReader);
            return new Message(messageObject, responseId);
        }

        public Message(string type) {
            this._messageObject = new MessageObject(type);
        }

        public Message(MessageObject messageObject) {
            this._messageObject = messageObject;
        }

        public Message(MessageObject messageObject, long responseId) {
            this._messageObject = messageObject;
            this.ResponseId = responseId;
        }

        public string MessageType => this._messageObject.Type;

        public MessageObject MessageObject => this._messageObject;

        public bool IsResponse => this.MessageType.EndsWith("Response");

        public T GetProperty<T>(string propertyName) where T : class => this._messageObject.GetProperty<T>(propertyName);

        public void AddProperty(string propertyName, string value) {
            this._messageObject.AddProperty(propertyName, value);
        }

        public void AddProperty(string propertyName, int value) {
            this._messageObject.AddProperty(propertyName, value);
        }

        public void Write(Stream outputStream) {
            using var binaryWriter = new BinaryWriter(outputStream);

            binaryWriter.Write(SignatureBytes);
            binaryWriter.Write(this.ResponseId);
            this._messageObject.Write(binaryWriter);
        }
    }
}
