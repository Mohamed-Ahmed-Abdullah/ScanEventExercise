using Newtonsoft.Json;
using DataModels.API;

namespace DataModels.JsonConverters
{
    // This is to handle situations where we receive EventTypes that are not defined in our EventType enum, so we fall them back to a specific "UNDEFINED".
    public class EventTypeJsonConverter : JsonConverter<EventType>
    {
        public override EventType ReadJson(JsonReader reader, 
                                           Type objectType, 
                                           EventType existingValue, 
                                           bool hasExistingValue, 
                                           JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string enumValue = reader.Value.ToString();
                if (Enum.TryParse(enumValue, true, out EventType result))
                {
                    return result;
                }
            }

            return EventType.UNDEFINED;
        }

        public override void WriteJson(JsonWriter writer, EventType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
