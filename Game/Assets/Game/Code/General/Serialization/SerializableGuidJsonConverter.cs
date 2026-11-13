using System;
using Newtonsoft.Json;

namespace Game.General
{
    public sealed class SerializableGuidJsonConverter : JsonConverter<SerializableGuid>
    {
        public override void WriteJson(JsonWriter writer, SerializableGuid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override SerializableGuid ReadJson(
            JsonReader reader,
            Type objectType,
            SerializableGuid existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String && Guid.TryParse((string)reader.Value, out var guid))
                return guid;

            reader.Skip();
            return SerializableGuid.Empty();
        }
    }
}
