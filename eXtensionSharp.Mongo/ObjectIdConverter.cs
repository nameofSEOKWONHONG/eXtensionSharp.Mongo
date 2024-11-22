using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace eXtensionSharp.Mongo;

public class ObjectIdConverter : JsonConverter<ObjectId>
{
    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        if (ObjectId.TryParse(stringValue, out var objectId))
        {
            return objectId;
        }
        throw new JsonException($"Unable to convert \"{stringValue}\" to ObjectId.");
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(ObjectId).IsAssignableFrom(typeToConvert);
    }
}