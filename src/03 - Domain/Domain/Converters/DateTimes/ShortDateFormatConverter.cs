using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Converters.DateTimes
{
    public class ShortDateFormatConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("d", new CultureInfo("pt-BR")));
        }
    }

   
}
