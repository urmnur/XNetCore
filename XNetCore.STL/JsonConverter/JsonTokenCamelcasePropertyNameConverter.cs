using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace XNetCore.STL
{
    class JsonTokenCamelcasePropertyNameConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(JToken).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jtoken = value as JToken;
            JsonReader reader = jtoken.CreateReader();
            JsonReaderToJsonWriter(reader, writer);
        }

        private void JsonReaderToJsonWriter(JsonReader reader, JsonWriter writer)
        {
            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.None:
                        break;
                    case JsonToken.StartObject:
                        writer.WriteStartObject();
                        break;
                    case JsonToken.StartArray:
                        writer.WriteStartArray();
                        break;
                    case JsonToken.PropertyName:
                        string propertyName = reader.Value.ToString();
                        writer.WritePropertyName(ConvertPropertyName(propertyName));
                        break;
                    case JsonToken.Comment:
                        writer.WriteComment((reader.Value != null) ? reader.Value.ToString() : null);
                        break;
                    case JsonToken.Integer:
                        writer.WriteValue(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
                        break;
                    case JsonToken.Float:
                        object value = reader.Value;
                        if (value is decimal)
                        {
                            writer.WriteValue((decimal)value);
                        }
                        else if (value is double)
                        {
                            writer.WriteValue((double)value);
                        }
                        else if (value is float)
                        {
                            writer.WriteValue((float)value);
                        }
                        else
                        {
                            writer.WriteValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        }
                        break;
                    case JsonToken.String:
                        writer.WriteValue(reader.Value.ToString());
                        break;
                    case JsonToken.Boolean:
                        writer.WriteValue(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
                        break;
                    case JsonToken.Null:
                        writer.WriteValue(string.Empty);
                        break;
                    case JsonToken.Undefined:
                        writer.WriteUndefined();
                        break;
                    case JsonToken.EndObject:
                        writer.WriteEndObject();
                        break;
                    case JsonToken.EndArray:
                        writer.WriteEndArray();
                        break;
                    case JsonToken.EndConstructor:
                        writer.WriteEndConstructor();
                        break;
                    case JsonToken.Date:
                        if (reader.Value is DateTimeOffset)
                        {
                            writer.WriteValue((DateTimeOffset)reader.Value);
                        }
                        else
                        {
                            writer.WriteValue(Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture));
                        }
                        break;
                    case JsonToken.Raw:
                        writer.WriteRawValue((reader.Value != null) ? reader.Value.ToString() : null);
                        break;
                    case JsonToken.Bytes:
                        if (reader.Value is Guid)
                        {
                            writer.WriteValue((Guid)reader.Value);
                        }
                        else
                        {
                            writer.WriteValue((byte[])reader.Value);
                        }
                        break;
                }
            } while (reader.Read());
        }

        private string ConvertPropertyName(string propertyName)
        {
            char[] chars = propertyName.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;
                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }
            return new string(chars);
        }
    }

}