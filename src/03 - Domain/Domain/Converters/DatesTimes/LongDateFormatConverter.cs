using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Converters.DatesTimes
{
    public class LongDateFormatConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "dd-MM-yyyy";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();

            if (DateTime.TryParseExact(
                dateString,
                DateFormat,
                new CultureInfo("pt-BR"),
                DateTimeStyles.None,
                out DateTime date))
            {
                return date;
            }
            else
            {
                throw new JsonException($"A data {dateString} não está no formato esperado {DateFormat}.");
            }
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Use o formato longo de data com cultura pt-BR ao escrever a data de volta ao JSON.
            writer.WriteStringValue(value.ToString("D", new CultureInfo("pt-BR")));
        }
    }
}