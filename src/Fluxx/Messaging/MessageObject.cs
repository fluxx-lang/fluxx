using System;
using System.Collections.Generic;
using System.IO;

namespace Faml.Messaging
{
    public class MessageObject
    {
        private readonly Dictionary<string, object> _properties;

        // Auto properties
        public string Type { get; }


        private enum PropertyType
        {
            Int32 = 1,
            Int64 = 2,
            Double = 3,
            Boolean = 4,
            String = 5,
            MessageObject = 6,
            ByteArray = 7,
            MessageObjectArray = 8
        }

        public MessageObject(string type)
        {
            this.Type = type;
            this._properties = new Dictionary<string, object>();
        }

        public MessageObject(string type, Dictionary<string, object> properties)
        {
            this.Type = type;
            this._properties = properties;
        }

        public object? GetPropertyIfExists(string propertyName) => !this._properties.TryGetValue(propertyName, out object value) ? null : value;

        public T GetProperty<T>(string propertyName)
        {
            object? value = this.GetPropertyIfExists(propertyName);
            if (value == null)
            {
                throw new InvalidOperationException($"Property not present: {propertyName}");
            }

            if (!(value is T typedValue))
                throw new InvalidOperationException(
                    $"Property {propertyName} is of type {value.GetType().FullName}, not expected type {typeof(T).FullName}");

            return typedValue;
        }

        public void AddProperty(string propertyName, string value)
        {
            this.DoAddProperty(propertyName, value);
        }

        public void AddProperty(string propertyName, int value)
        {
            this.DoAddProperty(propertyName, value);
        }

        private void DoAddProperty(string propertyName, object value)
        {
            this._properties.Add(propertyName, value);
        }

        public void Write(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.Type);

            int propertyCount = this._properties.Count;
            binaryWriter.Write(propertyCount);
            foreach (KeyValuePair<string, object> keyValuePair in this._properties)
            {
                binaryWriter.Write(keyValuePair.Key);
                this.WritePropertyValue(binaryWriter, keyValuePair.Key, keyValuePair.Value);
            }
        }

        private void WritePropertyValue(BinaryWriter binaryWriter, string name, object value)
        {
            if (value is int intValue)
            {
                this.WritePropertyType(binaryWriter, PropertyType.Int32);
                binaryWriter.Write(intValue);
            }
            else if (value is long longValue)
            {
                this.WritePropertyType(binaryWriter, PropertyType.Int64);
                binaryWriter.Write(longValue);
            }
            else if (value is double doubleValue)
            {
                this.WritePropertyType(binaryWriter, PropertyType.Double);
                binaryWriter.Write(doubleValue);
            }
            else if (value is bool booleanValue)
            {
                this.WritePropertyType(binaryWriter, PropertyType.Boolean);
                binaryWriter.Write(booleanValue);
            }
            else if (value is string stringValue)
            {
                this.WritePropertyType(binaryWriter, PropertyType.String);
                binaryWriter.Write(stringValue);
            }
            else if (value is MessageObject messageObjectValue)
            {
                this.WritePropertyType(binaryWriter, PropertyType.MessageObject);
                messageObjectValue.Write(binaryWriter);
            }
            else if (value is byte[] byteArrayValue)
            {
                this.WritePropertyType(binaryWriter, PropertyType.ByteArray);
                binaryWriter.Write(byteArrayValue.Length);
                binaryWriter.Write(byteArrayValue);
            }
            else if (value is MessageObject[] messageObjectArrayValue)
            {
                this.WritePropertyType(binaryWriter, PropertyType.MessageObjectArray);
                binaryWriter.Write(messageObjectArrayValue.Length);
                foreach (MessageObject messageObject in messageObjectArrayValue)
                {
                    messageObject.Write(binaryWriter);
                }
            }
            else
            {
                throw new NotSupportedException($"Unknown property type: {value.GetType().FullName}");
            }
        }

        private void WritePropertyType(BinaryWriter binaryWriter, PropertyType propertyType)
        {
            binaryWriter.Write((byte) propertyType);
        }

        public static MessageObject Read(BinaryReader binaryReader)
        {
            string messageType = binaryReader.ReadString();

            int propertyCount = binaryReader.ReadInt32();
            var properties = new Dictionary<string, object>();

            while (propertyCount-- > 0)
            {
                string name = binaryReader.ReadString();
                object value = ReadPropertyValue(binaryReader);

                properties.Add(name, value);
            }

            return new MessageObject(messageType, properties);
        }

        private static object ReadPropertyValue(BinaryReader binaryReader)
        {
            byte type = binaryReader.ReadByte();
            switch ((PropertyType) type)
            {
                case PropertyType.Int32:
                    return binaryReader.ReadInt32();
                case PropertyType.Int64:
                    return binaryReader.ReadInt64();
                case PropertyType.Double:
                    return binaryReader.ReadDouble();
                case PropertyType.Boolean:
                    return binaryReader.ReadBoolean();
                case PropertyType.String:
                    return binaryReader.ReadString();
                case PropertyType.MessageObject:
                    return Read(binaryReader);
                case PropertyType.ByteArray:
                {
                    int length = binaryReader.ReadInt32();
                    return binaryReader.ReadBytes(length);
                }

                case PropertyType.MessageObjectArray:
                {
                    int length = binaryReader.ReadInt32();
                    var list = new List<MessageObject>();
                    while (length-- > 0)
                        {
                            list.Add(Read(binaryReader));
                        }

                        return list.ToArray();
                }

                default:
                    throw new NotSupportedException($"Unknown property type: {type}");
            }
        }
    }
}
